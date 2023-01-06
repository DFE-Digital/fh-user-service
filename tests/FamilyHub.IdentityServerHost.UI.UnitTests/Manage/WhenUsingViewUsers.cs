using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;
using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Manage;

public class WhenUsingViewUsers
{
    private readonly ViewUsersModel _viewUsersModel;
    private readonly Mock<UserManager<ApplicationIdentityUser>> _userManagerMock;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly List<DisplayApplicationUser> _displayApplicationUser;

    public WhenUsingViewUsers()
    {
        _userManagerMock = new Mock<UserManager<ApplicationIdentityUser>>(
                        /* IUserStore<TUser> store */Mock.Of<IUserStore<ApplicationIdentityUser>>(),
                        /* IOptions<IdentityOptions> optionsAccessor */null,
                        /* IPasswordHasher<TUser> passwordHasher */null,
                        /* IEnumerable<IUserValidator<TUser>> userValidators */null,
                        /* IEnumerable<IPasswordValidator<TUser>> passwordValidators */null,
                        /* ILookupNormalizer keyNormalizer */null,
                        /* IdentityErrorDescriber errors */null,
                        /* IServiceProvider services */null,
                        /* ILogger<UserManager<TUser>> logger */null);

        _mockEmailSender = new Mock<IEmailSender>();


        Mock<IOrganisationRepository> organisationRepositoryMock = new Mock<IOrganisationRepository>();



        Mock<IApiService> apiServiceMock = new Mock<IApiService>();

        OpenReferralOrganisationWithServicesDto org = MockOrganisation.GetTestCountyCouncilDto();
        OpenReferralOrganisationWithServicesDto vcs = MockOrganisation.GetTestVCSDto();

        List<OpenReferralOrganisationDto> listVcs = new()
        {
            new OpenReferralOrganisationDto(
                    id: org.Id,
                    organisationType: org.OrganisationType,
                    name: org.Name ?? default!,
                    description: org.Description,
                    logo: org.Logo,
                    uri: org.Uri,
                    url: org.Url),
            new OpenReferralOrganisationDto(
                id: vcs.Id,
                organisationType: vcs.OrganisationType,
                name: vcs.Name ?? default!,
                description: vcs.Description,
                logo: vcs.Logo,
                uri: vcs.Uri,
                url: vcs.Url)
        };

        listVcs[0].AdministractiveDistrictCode = "XTEST";
        listVcs[1].AdministractiveDistrictCode = "XTEST";

        apiServiceMock.Setup(x => x.GetListOpenReferralOrganisations()).ReturnsAsync(listVcs);

        List<UserOrganisation> userOrganisations = new()
        {
            new UserOrganisation("1", "LAAdmin", org.Id),
            new UserOrganisation("2", "VCSAdmin", vcs.Id),
        };

        organisationRepositoryMock.Setup(x => x.GetUserOrganisations()).Returns(userOrganisations);

        List<ApplicationIdentityUser> applicationIdentities = new()
        {
            new ApplicationIdentityUser{ Id = "DfEAdmin", UserName = "DfEAdmin", Email = "DfEAdmin@email.com"  },
            new ApplicationIdentityUser{ Id = "LAAdmin", UserName = "BtlLAAdmin", Email = "BtlLAAdmin@email.com"  },
            new ApplicationIdentityUser{ Id = "VCSAdmin", UserName = "BtlVCSAdmin", Email = "BtlVCSAdmin@email.com"  }
        };

        _userManagerMock.Setup(x => x.Users).Returns(applicationIdentities.AsQueryable());

        //List<string> roles = new()
        //{
        //    "DfEAdmin", "LAAdmin", "VCSAdmin", "Professional"
        //};
        _userManagerMock.Setup(x => x.GetRolesAsync(applicationIdentities[0])).ReturnsAsync(new List<string>{ "DfEAdmin" });
        _userManagerMock.Setup(x => x.GetRolesAsync(applicationIdentities[1])).ReturnsAsync(new List<string> { "LAAdmin" });
        _userManagerMock.Setup(x => x.GetRolesAsync(applicationIdentities[2])).ReturnsAsync(new List<string> { "VCSAdmin" });

        _viewUsersModel = new ViewUsersModel(_userManagerMock.Object, _mockEmailSender.Object, organisationRepositoryMock.Object, apiServiceMock.Object);

        _displayApplicationUser = new List<DisplayApplicationUser>()
        {
            new DisplayApplicationUser()
                    {
                        Id = "LAAdmin",
                        UserName = "BtlLAAdmin",
                        Email = "BtlLAAdmin@email.com",
                        FullRoleNames = "Local authority administrator",
                        Roles = "LAAdmin",
                        OrganisationId = "56e62852-1b0b-40e5-ac97-54a67ea957dc",
                        OrganisationName = "Unit Test County Council",
                        LocalAuthority = "Unit Test County Council"
                    },
            new DisplayApplicationUser()
                    {
                        Id = "VCSAdmin",
                        UserName = "BtlVCSAdmin",
                        Email = "BtlVCSAdmin@email.com",
                        FullRoleNames = "Voluntary community organisation administrator",
                        Roles = "VCSAdmin",
                        OrganisationId = "66fcee88-038a-463f-b913-ea3e96884a6d",
                        OrganisationName = "Unit Test VCS",
                        LocalAuthority = "Unit Test County Council"
                    }
        };

        organisationRepositoryMock.Setup(x => x.GetUserOrganisationIdByUserId("LAAdmin")).Returns("56e62852-1b0b-40e5-ac97-54a67ea957dc");
        organisationRepositoryMock.Setup(x => x.GetUserOrganisationIdByUserId("VCSAdmin")).Returns("66fcee88-038a-463f-b913-ea3e96884a6d");
    }

    private void CreatePageContext(string role, string email)
    {
        var mockUrlHelper = MockHelpers.CreateMockUrlHelper();
        mockUrlHelper.Setup(h => h.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns("callbackUrl");
        var displayName = "User name";
        var identity = new GenericIdentity(displayName);
        identity.AddClaim(new Claim(ClaimTypes.Role, role));
        identity.AddClaim(new Claim(ClaimTypes.Email, email));
        var principle = new ClaimsPrincipal(identity);
        // use default context with user
        var httpContext = new DefaultHttpContext()
        {
            User = principle
        };

        //need these as well for the page context
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new PageActionDescriptor(), modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        // need page context for the page model
        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };

        _viewUsersModel.PageContext = pageContext;
        _viewUsersModel.Url = mockUrlHelper.Object;
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("X")]
    public async Task ThenGettingViewUsersPageWithLAAdmin(string pageNumber)
    {
        //Arrange
        CreatePageContext("LAAdmin", "BtlLAAdmin@email.com");
        _viewUsersModel.DfEAdmin = "Yes";
        _viewUsersModel.LAAdmin = "Yes";
        _viewUsersModel.VCSAdmin = "Yes";
        _viewUsersModel.Professional = "Yes";

        //Act
        await _viewUsersModel.OnGet(pageNumber);

        //Assert
        _viewUsersModel.Users.Items.Count().Should().Be(1);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("X")]
    public async Task ThenGettingViewUsersPageWithVCSAdmin(string pageNumber)
    {
        //Arrange
        CreatePageContext("VCSAdmin", "BtlVCSAdmin@email.com");
        _viewUsersModel.DfEAdmin = "Yes";
        _viewUsersModel.LAAdmin = "Yes";
        _viewUsersModel.VCSAdmin = "Yes";
        _viewUsersModel.Professional = "Yes";

        //Act
        await _viewUsersModel.OnGet(pageNumber);

        //Assert
        _viewUsersModel.Users.Items.Count().Should().Be(1);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("X")]
    public async Task ThenGettingViewUsersPageWithNoKeys(string pageNumber)
    {
        //Arrange
        CreatePageContext("LAAdmin", "BtlLAAdmin@email.com");
        

        //Act
        await _viewUsersModel.OnGet(pageNumber);

        //Assert
        _viewUsersModel.Users.Items.Count().Should().Be(1);
    }

    [Fact]
    public async Task ThenPostingViewUsersPage()
    {
        //Arrange
        CreatePageContext("LAAdmin", "BtlLAAdmin@email.com");
        _viewUsersModel.PageNumber = 1;
        _viewUsersModel.DfEAdmin = "Yes";
        _viewUsersModel.LAAdmin = "Yes";
        _viewUsersModel.VCSAdmin = "Yes";
        _viewUsersModel.Professional = "Yes";


        //Act
        await _viewUsersModel.OnPost();

        //Assert
        _viewUsersModel.Users.Items.Count().Should().Be(1);
    }

    [Fact]
    public async Task ThenGetingClearFilters()
    {
        //Arrange
        CreatePageContext("LAAdmin", "BtlLAAdmin@email.com");
        _viewUsersModel.PageNumber = 1;
        _viewUsersModel.DfEAdmin = "Yes";
        _viewUsersModel.LAAdmin = "Yes";
        _viewUsersModel.VCSAdmin = "Yes";
        _viewUsersModel.Professional = "Yes";


        //Act
        await _viewUsersModel.OnGetClearFilters();

        //Assert
        _viewUsersModel.DfEAdmin.Should().Be(default!);
        _viewUsersModel.LAAdmin.Should().Be(default!);
        _viewUsersModel.VCSAdmin.Should().Be(default!);
        _viewUsersModel.Professional.Should().Be(default!);
    }
}

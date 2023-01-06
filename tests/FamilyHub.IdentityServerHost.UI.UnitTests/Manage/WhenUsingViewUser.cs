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

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Manage;

public class WhenUsingViewUser
{
    private readonly ViewUserModel _viewUserModel;
    private readonly Mock<UserManager<ApplicationIdentityUser>> _userManagerMock;
    private readonly Mock<IEmailSender> _mockEmailSender;
    public WhenUsingViewUser()
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
            new ApplicationIdentityUser{ Id = "LAAdmin", UserName = "BtlLAAdmin", Email = "BtlLAAdmin@email.com"  },
            new ApplicationIdentityUser{ Id = "VCSAdmin", UserName = "BtlVCSAdmin", Email = "BtlVCSAdmin@email.com"  }
        };

        _userManagerMock.Setup(x => x.Users).Returns(applicationIdentities.AsQueryable());

        List<string> roles = new()
        {
            "DfEAdmin", "LAAdmin", "VCSAdmin", "Professional"
        };
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(roles);

        _viewUserModel = new ViewUserModel(_userManagerMock.Object, _mockEmailSender.Object, organisationRepositoryMock.Object, apiServiceMock.Object);

        var mockUrlHelper = MockHelpers.CreateMockUrlHelper();
        mockUrlHelper.Setup(h => h.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns("callbackUrl");
        var httpContext = new DefaultHttpContext()
        {
        };
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new PageActionDescriptor(), modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        // need page context for the page model
        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };

        _viewUserModel.PageContext = pageContext;
        _viewUserModel.Url = mockUrlHelper.Object;
    }

    private DisplayApplicationUser SetUpDisplayApplicationUser(string emailAddress)
    {
        DisplayApplicationUser displayApplicationUser;
        switch (emailAddress)
        {
            case "BtlLAAdmin@email.com":
                {
                    ApplicationIdentityUser applicationIdentityUser = new ApplicationIdentityUser { Id = "LAAdmin", UserName = "BtlLAAdmin", Email = "BtlLAAdmin@email.com" };
                    _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(applicationIdentityUser);

                    List<string> roles = new()
                    {
                        "LAAdmin"
                    };
                    _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(roles);

                    displayApplicationUser = new DisplayApplicationUser()
                    {
                        Id = "LAAdmin",
                        UserName = "BtlLAAdmin",
                        Email = "BtlLAAdmin@email.com",
                        FullRoleNames = "Local authority administrator",
                        Roles = "LAAdmin",
                        OrganisationId = "56e62852-1b0b-40e5-ac97-54a67ea957dc",
                        OrganisationName = "Unit Test County Council",
                        LocalAuthority = "Unit Test County Council"
                    };

                }

                break;

            case "BtlVCSAdmin@email.com":
                {
                    ApplicationIdentityUser applicationIdentityUser = new ApplicationIdentityUser { Id = "VCSAdmin", UserName = "BtlVCSAdmin", Email = "BtlVCSAdmin@email.com" };
                    _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(applicationIdentityUser);

                    List<string> roles = new()
                    {
                        "VCSAdmin"
                    };
                    _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(roles);

                    displayApplicationUser = new DisplayApplicationUser()
                    {
                        Id = "VCSAdmin",
                        UserName = "BtlVCSAdmin",
                        Email = "BtlVCSAdmin@email.com",
                        FullRoleNames = "Voluntary community organisation administrator",
                        Roles = "VCSAdmin",
                        OrganisationId = "66fcee88-038a-463f-b913-ea3e96884a6d",
                        OrganisationName = "Unit Test VCS",
                        LocalAuthority = "Unit Test County Council"
                    };
                }

                break;

            default:
                {
                    ApplicationIdentityUser applicationIdentityUser = default!;
                    _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(applicationIdentityUser);
                    _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(new List<string>());
                    displayApplicationUser = new DisplayApplicationUser()
                    {
                        Id = string.Empty,
                        UserName = string.Empty,
                        Email = string.Empty,
                        FullRoleNames = string.Empty,
                        Roles = string.Empty,
                        OrganisationId = default!,
                        OrganisationName = string.Empty,
                        LocalAuthority = string.Empty
                    };
                }

                break;
        }

        return displayApplicationUser;
    }

    [Theory]
    [InlineData("")]
    [InlineData("BtlLAAdmin@email.com")]
    [InlineData("BtlVCSAdmin@email.com")]
    public async Task ThenGettingViewUserPage(string emailAddress)
    {
        //Arrange
        DisplayApplicationUser displayApplicationUser = SetUpDisplayApplicationUser(emailAddress);
        
        //Act
        await _viewUserModel.OnGet(emailAddress);

        //Assert
        _viewUserModel.DisplayApplicationUser.Should().BeEquivalentTo(displayApplicationUser);
    }

    [Theory]
    [InlineData("BtlLAAdmin@email.com")]
    [InlineData("BtlVCSAdmin@email.com")]
    public async Task ThenGettingResetPassword(string emailAddress)
    {
        //Arrange
        DisplayApplicationUser displayApplicationUser = SetUpDisplayApplicationUser(emailAddress);
        _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync("UserToken");
        _mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        //Act
        var result = await _viewUserModel.OnGetResetPassword(emailAddress) as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/Account/ForgotPasswordConfirmation");
        
    }

    [Theory]
    [InlineData("")]
    public async Task ThenGettingResetPasswordWithNullUser(string emailAddress)
    {
        //Arrange
        DisplayApplicationUser displayApplicationUser = SetUpDisplayApplicationUser(emailAddress);
        _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync("UserToken");
        _mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        //Act
        var result = await _viewUserModel.OnGetResetPassword(emailAddress) as RedirectToPageResult;

        //Assert
        _viewUserModel.DisplayApplicationUser.Should().BeEquivalentTo(displayApplicationUser);

    }

    [Theory]
    [InlineData("BtlLAAdmin@email.com")]
    [InlineData("BtlVCSAdmin@email.com")]
    public async Task ThenGettingResetPasswordEmailThrowsException(string emailAddress)
    {
        //Arrange
        DisplayApplicationUser displayApplicationUser = SetUpDisplayApplicationUser(emailAddress);
        _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync("UserToken");
        _mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

        //Act
        var result = await _viewUserModel.OnGetResetPassword(emailAddress) as RedirectToPageResult;

        //Assert
        _viewUserModel.DisplayApplicationUser.Should().BeEquivalentTo(displayApplicationUser);

    }

    [Theory]
    [InlineData("BtlLAAdmin@email.com")]
    [InlineData("BtlVCSAdmin@email.com")]
    public async Task ThenGettingDeleteUser(string emailAddress)
    {
        //Arrange
        DisplayApplicationUser displayApplicationUser = SetUpDisplayApplicationUser(emailAddress);    

        //Act
        var result = await _viewUserModel.OnGetDeleteUser(emailAddress) as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/Account/DeleteUser");

    }

    [Theory]
    [InlineData("")]
    public async Task ThenGettingDeleteNullUser(string emailAddress)
    {
        //Arrange
        DisplayApplicationUser displayApplicationUser = SetUpDisplayApplicationUser(emailAddress);

        //Act
        var result = await _viewUserModel.OnGetDeleteUser(emailAddress) as RedirectToPageResult;

        //Assert
        _viewUserModel.DisplayApplicationUser.Should().BeEquivalentTo(displayApplicationUser);

    }
}

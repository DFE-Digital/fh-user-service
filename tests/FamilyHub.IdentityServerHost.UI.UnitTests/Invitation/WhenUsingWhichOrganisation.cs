using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using Serilog;
using System;
using System.Security.Claims;
using System.Security.Policy;
using System.Security.Principal;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Invitation;

public class WhenUsingWhichOrganisation
{
    private readonly WhichOrganisationModel _whichOrganisationModel;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    public WhenUsingWhichOrganisation()
    {
        var userManagerMock = new Mock<UserManager<ApplicationIdentityUser>>(
    /* IUserStore<TUser> store */Mock.Of<IUserStore<ApplicationIdentityUser>>(),
    /* IOptions<IdentityOptions> optionsAccessor */null,
    /* IPasswordHasher<TUser> passwordHasher */null,
    /* IEnumerable<IUserValidator<TUser>> userValidators */null,
    /* IEnumerable<IPasswordValidator<TUser>> passwordValidators */null,
    /* ILookupNormalizer keyNormalizer */null,
    /* IdentityErrorDescriber errors */null,
    /* IServiceProvider services */null,
    /* ILogger<UserManager<TUser>> logger */null);

        Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
        Mock<IApiService> mockApiService = new Mock<IApiService>();
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        mockRedisCacheService.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        mockRedisCacheService.Setup(x => x.RetrieveNewUser()).Returns(new Models.NewUser());

        _mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();

        var organisationId = Guid.NewGuid().ToString();
        mockApiService.Setup(x => x.GetListOpenReferralOrganisations()).ReturnsAsync(new List<OpenReferralOrganisationDto>
        {
            new OpenReferralOrganisationDto(
                id: organisationId,
                organisationType: new OrganisationTypeDto("1", "LA", "Local Authority"),
                name: "Bristol City Council",
                description: "Bristol City Council",
                logo: "logo",
                uri: "Uri",
                url: "Url"
                )
        });

        var userId = Guid.NewGuid().ToString();
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationIdentityUser() { Id = userId, Email = "User@email.com" });
        mockOrganisationRepository.Setup(x => x.GetAllUserOrganisationsByUserId(It.IsAny<string>())).Returns(new List<string> { organisationId });
        mockOrganisationRepository.Setup(x => x.GetUserOrganisationIdByUserId(It.IsAny<string>())).Returns(organisationId);

        
        _whichOrganisationModel = new(mockRedisCacheService.Object, mockOrganisationRepository.Object, mockApiService.Object, userManagerMock.Object, _mockRoleManager.Object);
    }

    private void CreatePageContext(string role)
    {
        var displayName = "User name";
        var identity = new GenericIdentity(displayName);
        identity.AddClaim(new Claim(ClaimTypes.Role, role));
        identity.AddClaim(new Claim(ClaimTypes.Email, $"{role}@email.com"));
        var principle = new ClaimsPrincipal(identity);
        // use default context with user
        var httpContext = new DefaultHttpContext()
        {
            User = principle
        };

        //need these as well for the page context
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        // need page context for the page model
        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };

        _whichOrganisationModel.PageContext = pageContext;
    }

    [Theory]
    [InlineData("DfEAdmin")]
    [InlineData("LAAdmin")]
    [InlineData("VCSAdmin")]
    public void ThenGettingWhichOrganisationPage(string role)
    {
        //Arrange
        var roles = new List<IdentityRole>()
        {
            new IdentityRole("DfEAdmin"),
            new IdentityRole("LAAdmin"),
            new IdentityRole("VCSAdmin"),
            new IdentityRole("Professional")

        };
        _mockRoleManager.Setup(x => x.Roles).Returns(roles.AsQueryable());
        CreatePageContext(role);

        //Act
        var exception = Record.ExceptionAsync(async () => await _whichOrganisationModel.OnGet());

        //Assert
        exception.IsCompletedSuccessfully.Should().BeTrue();
        _whichOrganisationModel.OrganisationSelectionList.Should().NotBeNull();
        _whichOrganisationModel.OrganisationSelectionList.Count.Should().Be(1);

    }

    [Theory]
    [InlineData("DfEAdmin")]
    [InlineData("LAAdmin")]
    [InlineData("VCSAdmin")]
    public async Task ThenReturningWhichOrganisationWithNoOrganisationSelectedPage(string role)
    {
        //Arrange
        var roles = new List<IdentityRole>()
        {
            new IdentityRole("DfEAdmin"),
            new IdentityRole("LAAdmin"),
            new IdentityRole("VCSAdmin"),
            new IdentityRole("Professional")

        };
        _mockRoleManager.Setup(x => x.Roles).Returns(roles.AsQueryable());
        CreatePageContext(role);
        _whichOrganisationModel.ModelState.AddModelError("OrganisationCode", "Required");

        //Act
        var result = await _whichOrganisationModel.OnPost() as RedirectToPageResult;

        //Assert
        _whichOrganisationModel.ModelState.IsValid.Should().BeFalse();
        

    }

    [Theory]
    [InlineData("DfEAdmin")]
    [InlineData("LAAdmin")]
    [InlineData("VCSAdmin")]
    public async Task ThenReturningWhichOrganisationWithOrganisationSelectedPage(string role)
    {
        //Arrange
        var roles = new List<IdentityRole>()
        {
            new IdentityRole("DfEAdmin"),
            new IdentityRole("LAAdmin"),
            new IdentityRole("VCSAdmin"),
            new IdentityRole("Professional")

        };
        _mockRoleManager.Setup(x => x.Roles).Returns(roles.AsQueryable());
        CreatePageContext(role);

        //Act
        var result = await _whichOrganisationModel.OnPost() as RedirectToPageResult;

        //Assert
        _whichOrganisationModel.ModelState.IsValid.Should().BeTrue();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/Invitation/WhatIsUsername");

    }
}

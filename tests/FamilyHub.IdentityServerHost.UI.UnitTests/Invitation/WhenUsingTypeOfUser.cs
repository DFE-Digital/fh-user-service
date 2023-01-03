using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation;
using FamilyHub.IdentityServerHost.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using NuGet.Configuration;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Invitation;

public class WhenUsingTypeOfUser
{
    private TypeOfUserModel _typeOfUserModel;
    private Mock<RoleManager<IdentityRole>> _mockRoleManager;
    public WhenUsingTypeOfUser()
    {
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        


        mockRedisCacheService.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        mockRedisCacheService.Setup(x => x.RetrieveNewUser()).Returns(new Models.NewUser());

        _mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();
        

        

        _typeOfUserModel = new(mockRedisCacheService.Object, _mockRoleManager.Object);

        

    }

    private void CreatePageContext(string role)
    {
        var displayName = "User name";
        var identity = new GenericIdentity(displayName);
        identity.AddClaim(new Claim(ClaimTypes.Role, role));
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

        _typeOfUserModel.PageContext = pageContext;
    }

    [Theory]
    [InlineData("DfEAdmin",4)]
    [InlineData("LAAdmin",3)]
    [InlineData("VCSAdmin",2)]
    public void ThenGettingTypeOfUserPage(string role, int availableRoleNumber)
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
        var exception = Record.Exception(() => _typeOfUserModel.OnGet());

        //Assert
        exception.Should().BeNull();
        _typeOfUserModel.AvailableRoles.Count.Should().Be(availableRoleNumber);

    }

    [Theory]
    [InlineData("DfEAdmin", 4)]
    [InlineData("LAAdmin", 3)]
    [InlineData("VCSAdmin", 2)]
    public void ThenReturningPageWithNoRoleSelection(string role, int availableRoleNumber)
    {
        //Arrange
        _typeOfUserModel.RoleSelection = string.Empty;
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
        var result = _typeOfUserModel.OnPost() as RedirectToPageResult;

        //Assert
        _typeOfUserModel.ModelState.IsValid.Should().BeFalse();
        _typeOfUserModel.AvailableRoles.Count.Should().Be(availableRoleNumber);
    }

    [Theory]
    [InlineData("DfEAdmin", "/Invitation/WhatIsUsername")]
    [InlineData("LAAdmin", "/Invitation/WhichOrganisation")]
    [InlineData("VCSAdmin", "/Invitation/WhichOrganisation")]
    public void ThenReturningPageWithRoleSelection_ShouldBeSuccessful(string role, string pageName)
    {
        //Arrange
        _typeOfUserModel.RoleSelection = role;
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
        var result = _typeOfUserModel.OnPost() as RedirectToPageResult;

        //Assert
        _typeOfUserModel.ModelState.IsValid.Should().BeTrue();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be(pageName);
        
        
        
    }
}

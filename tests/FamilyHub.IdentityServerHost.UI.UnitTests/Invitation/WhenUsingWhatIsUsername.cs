using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation;
using FamilyHub.IdentityServerHost.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Invitation;

public class WhenUsingWhatIsUsername
{
    private readonly WhatIsUsernameModel _whatIsUsernameModel;
    public WhenUsingWhatIsUsername()
    {
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        mockRedisCacheService.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        mockRedisCacheService.Setup(x => x.RetrieveNewUser()).Returns(new Models.NewUser() { FullName = "FullName"});
        _whatIsUsernameModel = new(mockRedisCacheService.Object);
    }

    [Fact]
    public void ThenGettingWhatIsUsernamePage()
    {
        //Arrange

        //Act
        var exception = Record.Exception(() => _whatIsUsernameModel.OnGet());

        //Assert
        exception.Should().BeNull();
        _whatIsUsernameModel?.NewUser?.FullName.Should().Be("FullName");

    }
    [Fact]
    public void ThenReturningPageWithNoUsername()
    {
        //Arrange
        _whatIsUsernameModel.NewUser = new Models.NewUser() { FullName = string.Empty };
        _whatIsUsernameModel.ModelState.AddModelError("FullName", "Required");


        //Act
        var result = _whatIsUsernameModel.OnPost() as RedirectToPageResult;


        //Assert
        _whatIsUsernameModel.ModelState.IsValid.Should().BeFalse();
        
    }

    [Fact]
    public void ThenReturningPageWithUsername()
    {
        //Arrange
        _whatIsUsernameModel.NewUser = new Models.NewUser() { FullName = "FullName" };
        

        //Act
        var result = _whatIsUsernameModel.OnPost() as RedirectToPageResult;


        //Assert
        _whatIsUsernameModel.ModelState.IsValid.Should().BeTrue();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/Invitation/WhatIsEmailAddress");

    }
}

using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation;
using FamilyHub.IdentityServerHost.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Invitation;

public class WhenUsingWhatIsEmailAddress
{
    private readonly WhatIsEmailAddressModel _whatIsEmailAddressModel;
    public WhenUsingWhatIsEmailAddress()
    {
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        mockRedisCacheService.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        mockRedisCacheService.Setup(x => x.RetrieveNewUser()).Returns(new Models.NewUser() { EmailAddress = "someone@aol.com", FullName = "FullName"});
        _whatIsEmailAddressModel = new(mockRedisCacheService.Object);
    }

    [Fact]
    public void ThenGettingWhatIsEmailAddressPage()
    {
        //Arrange

        //Act
        var exception = Record.Exception(() => _whatIsEmailAddressModel.OnGet());

        //Assert
        exception.Should().BeNull();
        _whatIsEmailAddressModel?.NewUser?.EmailAddress.Should().Be("someone@aol.com");
        _whatIsEmailAddressModel?.NewUser?.FullName.Should().Be("FullName");

    }
    [Fact]
    public void ThenReturningPageWithNoEmail()
    {
        //Arrange
        _whatIsEmailAddressModel.NewUser = new Models.NewUser() { FullName = string.Empty };
        _whatIsEmailAddressModel.ModelState.AddModelError("EmailAddress", "Required");


        //Act
        var result = _whatIsEmailAddressModel.OnPost() as RedirectToPageResult;


        //Assert
        _whatIsEmailAddressModel.ModelState.IsValid.Should().BeFalse();
        
    }

    [Fact]
    public void ThenReturningPageWithEmailAddress()
    {
        //Arrange
        _whatIsEmailAddressModel.NewUser = new Models.NewUser() { EmailAddress = "someone@aol.com", FullName = "FullName" };
        

        //Act
        var result = _whatIsEmailAddressModel.OnPost() as RedirectToPageResult;


        //Assert
        _whatIsEmailAddressModel.ModelState.IsValid.Should().BeTrue();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/Invitation/CheckAccountDetails");

    }
}

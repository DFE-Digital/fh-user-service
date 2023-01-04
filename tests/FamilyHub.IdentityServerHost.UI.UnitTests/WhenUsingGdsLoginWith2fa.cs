using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Account;
using FamilyHub.IdentityServerHost.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests;

public class WhenUsingGdsLoginWith2fa
{
    

    private readonly Mock<SignInManager<ApplicationIdentityUser>> _signInManagerMock;
    private readonly Mock<UserManager<ApplicationIdentityUser>>? _userManagerMock;
    private LoginWith2faModel _loginWith2FaModel;
    public WhenUsingGdsLoginWith2fa()
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

        _signInManagerMock = new Mock<SignInManager<ApplicationIdentityUser>>(
            _userManagerMock.Object,
            /* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
            /* IUserClaimsPrincipalFactory<TUser> claimsFactory */Mock.Of<IUserClaimsPrincipalFactory<ApplicationIdentityUser>>(),
            /* IOptions<IdentityOptions> optionsAccessor */null,
            /* ILogger<SignInManager<TUser>> logger */null,
            /* IAuthenticationSchemeProvider schemes */null,
            /* IUserConfirmation<TUser> confirmation */null);

        Mock<ILogger<LoginWith2faModel>> mockLogger = new Mock<ILogger<LoginWith2faModel>>();

        _loginWith2FaModel = new(_signInManagerMock.Object, _userManagerMock.Object, mockLogger.Object);
    }

    [Fact]
    public void ThenGettingLoginWith2faWhenUserIsNullPage()
    {
        //Arrange
        ApplicationIdentityUser user = default!;
        _signInManagerMock.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(user);

        //Act
        var exception = Record.ExceptionAsync(async () => await _loginWith2FaModel.OnGetAsync(It.IsAny<bool>(), It.IsAny<string>()));
        exception.Wait();
        

        //Assert
        exception.Result.Message.Should().Be("Unable to load two-factor authentication user.");
    }

    [Fact]
    public void ThenGettingLoginWith2faWhenUserIsReturnedPage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        _signInManagerMock.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(user);

        //Act
        var exception = Record.ExceptionAsync(async () => await _loginWith2FaModel.OnGetAsync(It.IsAny<bool>(), It.IsAny<string>()));
        exception.Wait();

        //Assert
        exception.IsCompletedSuccessfully.Should().BeTrue();
    }

    [Fact]
    public async Task ThenSubmittingLoginWith2faWithInvalidModePage()
    {
        //Arrange
        _loginWith2FaModel.ModelState.AddModelError("TwoFactorCode", "TwoFactorCode is required");

        //Act
        var result = await _loginWith2FaModel.OnPostAsync(It.IsAny<bool>(), It.IsAny<string>()) as RedirectToPageResult;

        //Assert
        _loginWith2FaModel.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ThenSubmittingLoginWith2faWithValidModePage()
    {
        //Arrange
        _loginWith2FaModel.Input = new LoginWith2faModel.InputModel
        {
            TwoFactorCode = "ABC123"
        };
        ApplicationIdentityUser user = new();
        MySignInResult mySignInResult = new MySignInResult();
        mySignInResult.SetSucceeded(true);
        _signInManagerMock.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.TwoFactorAuthenticatorSignInAsync(It.IsAny<string>(),It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(mySignInResult);
        _userManagerMock?.Setup(x => x.GetUserIdAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(user.Id);

        //Act
        var result = await _loginWith2FaModel.OnPostAsync(true, "/Index") as RedirectToPageResult;

        //Assert
        result.Should().BeNull();
        
    }

    [Fact]
    public async Task ThenSubmittingLoginWith2faWithValidModeAndIsLockedOutPage()
    {
        //Arrange
        _loginWith2FaModel.Input = new LoginWith2faModel.InputModel
        {
            TwoFactorCode = "ABC123"
        };
        ApplicationIdentityUser user = new();
        MySignInResult mySignInResult = new MySignInResult();
        mySignInResult.SetSucceeded(false);
        mySignInResult.SetLockedOut(true);
        _signInManagerMock.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.TwoFactorAuthenticatorSignInAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(mySignInResult);
        _userManagerMock?.Setup(x => x.GetUserIdAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(user.Id);

        //Act
        var result = await _loginWith2FaModel.OnPostAsync(true, "/Index") as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("./Lockout");

    }

    [Fact]
    public async Task ThenSubmittingLoginWith2faWithValidModeAndHasNotSucceededPage()
    {
        //Arrange
        _loginWith2FaModel.Input = new LoginWith2faModel.InputModel
        {
            TwoFactorCode = "ABC123"
        };
        ApplicationIdentityUser user = new();
        MySignInResult mySignInResult = new MySignInResult();
        mySignInResult.SetSucceeded(false);
        _signInManagerMock.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.TwoFactorAuthenticatorSignInAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(mySignInResult);
        _userManagerMock?.Setup(x => x.GetUserIdAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(user.Id);

        //Act
        var result = await _loginWith2FaModel.OnPostAsync(true, "/Index") as RedirectToPageResult;

        //Assert
        _loginWith2FaModel.ModelState.IsValid.Should().BeFalse();
    }
}

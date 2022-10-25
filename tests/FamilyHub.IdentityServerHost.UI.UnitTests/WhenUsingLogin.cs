using FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;
using FamilyHub.IdentityServerHost.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests;
public class WhenUsingLogin
{
    private LoginModel _loginModel;
    private Mock<IAuthenticationService> _authServiceMock;
    private Mock<SignInManager<ApplicationIdentityUser>> _signInManagerMock;

    public WhenUsingLogin()
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

        _signInManagerMock = new Mock<SignInManager<ApplicationIdentityUser>>(
            userManagerMock.Object,
            /* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
            /* IUserClaimsPrincipalFactory<TUser> claimsFactory */Mock.Of<IUserClaimsPrincipalFactory<ApplicationIdentityUser>>(),
            /* IOptions<IdentityOptions> optionsAccessor */null,
            /* ILogger<SignInManager<TUser>> logger */null,
            /* IAuthenticationSchemeProvider schemes */null,
            /* IUserConfirmation<TUser> confirmation */null);

        //Mock<SignInManager<ApplicationIdentityUser>> mockSignInManager = new Mock<SignInManager<ApplicationIdentityUser>>();    
        Mock<ILogger<LoginModel>> mockLogger = new Mock<ILogger<LoginModel>>();
        _loginModel = new LoginModel(_signInManagerMock.Object, mockLogger.Object);

        var mockHttpContext = new Mock<HttpContext>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _authServiceMock.Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult((object)default!));
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(_authServiceMock.Object);
        mockHttpContext.Setup(x => x.RequestServices)
          .Returns(serviceProviderMock.Object);

        _loginModel.PageContext.HttpContext = mockHttpContext.Object;

        _loginModel.OnGetAsync("~/").Wait();
    }

    [Fact]
    public async Task WhenEmailAndPasswordIsEmpty()
    {
        // Arrange
        _loginModel.Input = new FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account.LoginModel.InputModel();
        _loginModel.Input.Email = string.Empty;
        _loginModel.Input.Password = string.Empty;
        _signInManagerMock.Setup(x => x.GetExternalAuthenticationSchemesAsync()).Returns(Task.FromResult(new List<AuthenticationScheme>().AsEnumerable()));
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(new Microsoft.AspNetCore.Identity.SignInResult()));

        //Act
        var result = await _loginModel.OnPostAsync("~/") as PageResult;

        // Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.Page.Should().BeNull();

    }

    [Fact]
    public async Task WhenEmailAndPasswordAreOK_RedirectUrlIsReturned()
    {
        // Arrange
        _loginModel.Input = new FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account.LoginModel.InputModel();
        _loginModel.Input.Email = "someone@email.com";
        _loginModel.Input.Password = "Pas$123";

        Microsoft.AspNetCore.Identity.SignInResult signInResult = new();
        if (typeof(Microsoft.AspNetCore.Identity.SignInResult) != null && typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("Succeeded") != null)
        {
            var signInResultClass = typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("Succeeded");
            if (signInResultClass != null)
                signInResultClass.SetValue(signInResult, true, null);
        }
        

        _signInManagerMock.Setup(x => x.GetExternalAuthenticationSchemesAsync()).Returns(Task.FromResult(new List<AuthenticationScheme>().AsEnumerable()));
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(signInResult));

        //Act
        var result = await _loginModel.OnPostAsync("~/") as LocalRedirectResult;

        // Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.Url.Should().Be("~/");


    }

    [Fact]
    public async Task WhenEmailAndPasswordAreOKAndRequiresTwoFactor_RedirectUrlIsReturned()
    {
        // Arrange
        _loginModel.Input = new FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account.LoginModel.InputModel();
        _loginModel.Input.Email = "someone@email.com";
        _loginModel.Input.Password = "Pas$123";

        Microsoft.AspNetCore.Identity.SignInResult signInResult = new();
        if (typeof(Microsoft.AspNetCore.Identity.SignInResult) != null && typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("Succeeded") != null)
        {
            var signInResultClass = typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("RequiresTwoFactor");
            if (signInResultClass != null)
                signInResultClass.SetValue(signInResult, true, null);
        }


        _signInManagerMock.Setup(x => x.GetExternalAuthenticationSchemesAsync()).Returns(Task.FromResult(new List<AuthenticationScheme>().AsEnumerable()));
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(signInResult));

        //Act
        var result = await _loginModel.OnPostAsync("~/") as RedirectToPageResult;

        // Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("./LoginWith2fa");
    }

    [Fact]
    public async Task WhenEmailAndPasswordAreOKAndIsLockedOut_RedirectUrlIsReturned()
    {
        // Arrange
        _loginModel.Input = new FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account.LoginModel.InputModel();
        _loginModel.Input.Email = "someone@email.com";
        _loginModel.Input.Password = "Pas$123";

        Microsoft.AspNetCore.Identity.SignInResult signInResult = new();
        if (typeof(Microsoft.AspNetCore.Identity.SignInResult) != null && typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("Succeeded") != null)
        {
            var signInResultClass = typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("IsLockedOut");
            if (signInResultClass != null)
                signInResultClass.SetValue(signInResult, true, null);
        }


        _signInManagerMock.Setup(x => x.GetExternalAuthenticationSchemesAsync()).Returns(Task.FromResult(new List<AuthenticationScheme>().AsEnumerable()));
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(signInResult));

        //Act
        var result = await _loginModel.OnPostAsync("~/") as RedirectToPageResult;

        // Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("./Lockout");
    }
}
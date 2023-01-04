using FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;
using FamilyHub.IdentityServerHost.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.MyAccount;

public class WhenChangePassword
{
    private readonly Mock<SignInManager<ApplicationIdentityUser>> _signInManagerMock;
    private readonly Mock<UserManager<ApplicationIdentityUser>> _userManagerMock;
    private readonly ChangePasswordModel _changePasswordModel;

    private class MyIdentityResult : IdentityResult
    {
        public void SetSuccessful()
        {
            this.Succeeded = true;
        }
    }
        

    public WhenChangePassword()
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

        Mock<ILogger<ChangePasswordModel>> logger = new();

        _changePasswordModel = new ChangePasswordModel(_userManagerMock.Object, _signInManagerMock.Object, logger.Object);
    }

    [Fact]
    public async Task ThenGettingChangePasswordWithNullUserPage()
    {
        //Arrange
        ApplicationIdentityUser user = default!;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        //Act
        var result = await _changePasswordModel.OnGetAsync() as NotFoundObjectResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.Value.Should().Be("Unable to load user with ID ''.");
    }

    [Fact]
    public async Task ThenGettingChangePasswordPageWithUserButNoPassword()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.HasPasswordAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(false);

        //Act
        var result = await _changePasswordModel.OnGetAsync() as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/Account/SetPassword");
    }

    [Fact]
    public async Task ThenGettingChangePasswordPageWithUser()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.HasPasswordAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(true);

        //Act
        var result = await _changePasswordModel.OnGetAsync() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
    }

    [Fact]
    public async Task ThenPostingChangePasswordPageWithNullUser()
    {
        //Arrange
        ApplicationIdentityUser user = default!;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        //Act
        var result = await _changePasswordModel.OnPostAsync() as NotFoundObjectResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.Value.Should().Be("Unable to load user with ID ''.");
    }

    [Fact]
    public async Task ThenPostingChangePasswordPageWithInvalidModel()
    {
        //Arrange
        _changePasswordModel.ModelState.AddModelError("NewPassword", "NewPassword Required");

        //Act
        var result = await _changePasswordModel.OnPostAsync() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _changePasswordModel.ModelState.IsValid.Should().BeFalse();
        
    }

    [Fact]
    public async Task ThenPostingChangePasswordPageWhichFails()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        List<IdentityError> errors = new()
        {
            new IdentityError
            {
                Code = "UnitTest",
                Description = "UnitTest"
            }
        };
        
        var identityResult = IdentityResult.Failed(errors.ToArray());
        _userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(identityResult);
        _changePasswordModel.Input = new ChangePasswordModel.InputModel
        {
            OldPassword = "OldPassword",
            NewPassword = "NewPassword",
            ConfirmPassword = "NewPassword"
        };

        //Act
        var result = await _changePasswordModel.OnPostAsync() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _changePasswordModel.ModelState.IsValid.Should().BeFalse();

    }

    [Fact]
    public async Task ThenPostingChangePasswordPage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        MyIdentityResult myIdentityResult = new();
        myIdentityResult.SetSuccessful(); 
        _userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(myIdentityResult);
        _changePasswordModel.Input = new ChangePasswordModel.InputModel
        {
            OldPassword = "OldPassword",
            NewPassword = "NewPassword",
            ConfirmPassword = "NewPassword"
        };
        _signInManagerMock.Setup(x => x.RefreshSignInAsync(It.IsAny<ApplicationIdentityUser>()));

        //Act
        var result = await _changePasswordModel.OnPostAsync() as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _changePasswordModel.StatusMessage.Should().Be("Your password has been changed.");
        result.PageName.Should().Be("/MyAccount/ConfirmPasswordChanged");

    }
}

using FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.MyAccount;

public class WhenUsingChangeName
{
    private readonly ChangeNameModel _changeNameModel;
    private readonly Mock<UserManager<ApplicationIdentityUser>> _userManagerMock;
    private readonly Mock<IApplicationDbContext> _applicationDbContextMock;
    public WhenUsingChangeName()
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

        _applicationDbContextMock = new Mock<IApplicationDbContext>();

        _changeNameModel = new ChangeNameModel(_userManagerMock.Object, _applicationDbContextMock.Object);
    }

    [Fact]
    public async Task ThenGettingChangeNameWithNullUserPage()
    {
        //Arrange
        ApplicationIdentityUser user = default!;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        //Act
        var result = await _changeNameModel.OnGet() as NotFoundObjectResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.Value.Should().Be("Unable to load user with ID ''.");
    }

    [Fact]
    public async Task ThenGettingChangeNamePage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetUserNameAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync("User");
        _applicationDbContextMock.Setup(x => x.GetFullName(It.IsAny<string>())).Returns(string.Empty);

        //Act
        var result = await _changeNameModel.OnGet() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _changeNameModel.Username.Should().Be("User");
    }

    [Fact]
    public async Task ThenPostingChangeNamePageWithInvalidModel()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _changeNameModel.ModelState.AddModelError("Username", "Username is required");

        //Act
        var result = await _changeNameModel.OnPost() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _changeNameModel.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ThenPostingChangeNamePageWithValidModelButUnableToSetName()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetUserNameAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync("User");
        _applicationDbContextMock.Setup(x => x.GetFullName(It.IsAny<string>())).Returns(string.Empty);
        _applicationDbContextMock.Setup(x => x.SetFullNameAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        //Act
        var result = await _changeNameModel.OnPost() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _changeNameModel.StatusMessage.Should().Be("Error changing user name.");
    }

    [Fact]
    public async Task ThenPostingChangeNamePageWithValidModel()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetUserNameAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync("User");
        _applicationDbContextMock.Setup(x => x.GetFullName(It.IsAny<string>())).Returns(string.Empty);
        _applicationDbContextMock.Setup(x => x.SetFullNameAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        //Act
        var result = await _changeNameModel.OnPost() as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/MyAccount/ConfirmNameChanged");


    }

}

    

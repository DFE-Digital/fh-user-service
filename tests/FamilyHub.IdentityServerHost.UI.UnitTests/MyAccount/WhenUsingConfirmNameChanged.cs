using FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.MyAccount;

public class WhenUsingConfirmNameChanged
{
    private readonly ConfirmNameChangedModel _confirmNameChangedModel;
    private readonly Mock<UserManager<ApplicationIdentityUser>> _userManagerMock;
    private readonly Mock<IApplicationDbContext> _applicationDbContextMock;
    public WhenUsingConfirmNameChanged()
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

        _confirmNameChangedModel = new ConfirmNameChangedModel(_userManagerMock.Object, _applicationDbContextMock.Object);
    }

    [Fact]
    public async Task ThenGettingConfirmNameChangedPageWithNullUser()
    {
        //Arrange
        ApplicationIdentityUser user = default!;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        //Act
        var result = await _confirmNameChangedModel.OnGet() as NotFoundObjectResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.Value.Should().Be("Unable to load user with ID ''.");
    }

    [Fact]
    public async Task ThenGettingConfirmNameChangedPagePage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetUserNameAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync("User");
        _applicationDbContextMock.Setup(x => x.GetFullName(It.IsAny<string>())).Returns(string.Empty);

        //Act
        var result = await _confirmNameChangedModel.OnGet() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _confirmNameChangedModel.Username.Should().Be("User");
    }
}

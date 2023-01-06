using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;
using FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Manage;

public  class WhenUsingHomePage
{
    private readonly HomepageModel _homepageModel;
    private readonly Mock<UserManager<ApplicationIdentityUser>> _userManagerMock;
    private readonly Mock<IApplicationDbContext> _applicationDbContextMock;
    public WhenUsingHomePage()
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

        _homepageModel = new HomepageModel(_userManagerMock.Object, _applicationDbContextMock.Object);
    }

    [Fact]
    public void ThenGettingHomePage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _applicationDbContextMock.Setup(x => x.GetFullName(It.IsAny<string>())).Returns("UnitTestUser");

        //Act
        _homepageModel.OnGet();

        //Assert
        _homepageModel.UserName.Should().Be("UnitTestUser");
    }
}

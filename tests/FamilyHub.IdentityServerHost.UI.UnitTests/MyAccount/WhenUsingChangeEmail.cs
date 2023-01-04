using FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;
using FamilyHub.IdentityServerHost.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.MyAccount;

public class WhenUsingChangeEmail
{
    private readonly Mock<SignInManager<ApplicationIdentityUser>> _signInManagerMock;
    private readonly Mock<UserManager<ApplicationIdentityUser>> _userManagerMock;
    private readonly ChangeEmailModel _changeEmailModel;
    public WhenUsingChangeEmail()
    {
        Mock<IEmailSender> mockEmailSender = new Mock<IEmailSender>();
        mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

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

        _userManagerMock.Setup(x => x.GetEmailAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync("someone@aol.com");
        _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(true);

        _changeEmailModel = new ChangeEmailModel(_userManagerMock.Object, _signInManagerMock.Object, mockEmailSender.Object);

        var mockUrlHelper = MockHelpers.CreateMockUrlHelper();
        mockUrlHelper.Setup(h => h.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns("callbackUrl");
        var httpContext = new DefaultHttpContext()
        {
        };
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new PageActionDescriptor(), modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        // need page context for the page model
        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };

        _changeEmailModel.PageContext = pageContext;
        _changeEmailModel.Url = mockUrlHelper.Object;
    }

    [Fact]
    public async Task ThenGettingChangeEmailWithNullUserPage()
    {
        //Arrange
        ApplicationIdentityUser user = default!;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        //Act
        var result = await _changeEmailModel.OnGetAsync() as NotFoundObjectResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.Value.Should().Be("Unable to load user with ID ''.");
    }

    [Fact]
    public async Task ThenGettingChangeEmailWithUserPage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        //Act
        var result = await _changeEmailModel.OnGetAsync() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
    }

    [Fact]
    public async Task ThenPostingChangeEmailWithNullUserPage()
    {
        //Arrange
        ApplicationIdentityUser user = default!;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        //Act
        var result = await _changeEmailModel.OnPostChangeEmailAsync() as NotFoundObjectResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.Value.Should().Be("Unable to load user with ID ''.");
    }

    [Fact]
    public async Task ThenPostingChangeEmailWithUserPage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Id = "ABC123";
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetUserIdAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(user.Id);
        _userManagerMock.Setup(x => x.GenerateChangeEmailTokenAsync(It.IsAny<ApplicationIdentityUser>(),It.IsAny<string>())).ReturnsAsync("MyToken");
        _changeEmailModel.Input = new ChangeEmailModel.InputModel()
        {
            NewEmail = "Joe.Blogs@aol.com"
        };

        //Act
        var result = await _changeEmailModel.OnPostChangeEmailAsync() as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
    }

    [Fact]
    public async Task ThenPostingChangeEmailWithIdenticalEmailAddressUserPage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Id = "ABC123";
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _changeEmailModel.Input = new ChangeEmailModel.InputModel()
        {
            NewEmail = user.Email
        };

        //Act
        var result = await _changeEmailModel.OnPostChangeEmailAsync() as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _changeEmailModel.StatusMessage.Should().Be("Your email is unchanged.");
    }

    [Fact]
    public async Task ThenPostingChangeEmailWithBlankEmailAddressUserPage()
    {
        //Arrange
        ApplicationIdentityUser user = new();
        user.Id = "ABC123";
        user.Email = "someone@aol.com";
        user.EmailConfirmed = true;
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        // _userManagerMock.Setup(x => x.GetUserIdAsync(It.IsAny<ApplicationIdentityUser>())).ReturnsAsync(user.Id);
        // _userManagerMock.Setup(x => x.GenerateChangeEmailTokenAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<string>())).ReturnsAsync("MyToken");
        _changeEmailModel.Input = new ChangeEmailModel.InputModel()
        {
            NewEmail = default!
        };
        _changeEmailModel.ModelState.AddModelError("NewEmail","Email Address is Required");

        //Act
        var result = await _changeEmailModel.OnPostChangeEmailAsync() as PageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _changeEmailModel.ModelState.IsValid.Should().BeFalse();
        
    }

}

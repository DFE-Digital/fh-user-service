using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation;
using FamilyHub.IdentityServerHost.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Invitation;

public class WhenUsingCheckAccountDetails
{
    private readonly CheckAccountDetailsModel _checkAccountDetailsModel;
    
    public WhenUsingCheckAccountDetails()
    {
        Mock<IEmailSender> mockEmailSender = new Mock<IEmailSender>();
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        mockRedisCacheService.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        mockRedisCacheService.Setup(x => x.RetrieveNewUser()).Returns(new Models.NewUser { FullName = "Full Name", EmailAddress = "someone@aol.com", OrganisationId = Guid.NewGuid().ToString(), OrganisationName = "OrganisationName", Role = "DfEAdmin" });
        mockRedisCacheService.Setup(x => x.RetrieveLastPageName()).Returns("~/");
        mockRedisCacheService.Setup(x => x.ResetNewUser());

        mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("InvitationKey", "7PCQDtCtKXhKfieW")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _checkAccountDetailsModel = new(mockRedisCacheService.Object, configuration, mockEmailSender.Object);
    }

    [Fact]
    public void ThenGettingCheckAccountDetailsPage()
    {
        //Arrange

        //Act
        var exception = Record.Exception(() => _checkAccountDetailsModel.OnGet());

        //Assert
        exception.Should().BeNull();
    }

    [Fact]
    public async Task ThenPostingCheckAccountDetailsPage()
    {
        //Arrange
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

        
        _checkAccountDetailsModel.PageContext = pageContext;
        _checkAccountDetailsModel.Url = mockUrlHelper.Object;

        //Act
        var result = await _checkAccountDetailsModel.OnPost() as RedirectToPageResult;


        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/Invitation/Confirmation");
    }
}

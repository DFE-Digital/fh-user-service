using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Configuration;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectoryAdminUi.Ui.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;


namespace FamilyHub.IdentityServerHost.Extensions;

public static class HtmlHelperExtensions
{
    public static IHeaderViewModel GetHeaderViewModel(this IHtmlHelper html, bool hideMenu = false)
    {
        bool isAbleToCallAuthenticatedServices = IsAbleToCallAuthenticatedServices(html);
        var externalLinks = (html.ViewContext.HttpContext.RequestServices.GetService(typeof(IOptions<ExternalLinksConfiguration>)) as IOptions<ExternalLinksConfiguration>)?.Value;
        var authConfig = (html.ViewContext.HttpContext.RequestServices.GetService(typeof(IOptions<IdentityServerOptions>)) as IOptions<IdentityServerOptions>)?.Value;
        var requestRoot = html.ViewContext.HttpContext.Request.GetRequestUrlRoot();
        var requestPath = html.ViewContext.HttpContext.Request.Path;
        var hashedAccountId = html.ViewContext.RouteData.Values["accountId"]?.ToString();
        ConfigurationManager? configurationManager = html.ViewContext.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as ConfigurationManager;

        bool UseOriginalCode = true;
        if (configurationManager != null)
        {
            UseOriginalCode = configurationManager.GetValue<bool>("UseOriginalCode");
        }

        string userName = string.Empty;
        UserManager<ApplicationIdentityUser>? userManager = html.ViewContext.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationIdentityUser>)) as UserManager<ApplicationIdentityUser>;
        if (userManager != null)
        {
            userName = userManager.GetUserName(html.ViewContext.HttpContext.User);
        }

        var headerModel = new HeaderViewModel(new HeaderConfiguration
        {
            AuthenticationAuthorityUrl = authConfig?.BaseAddress ?? string.Empty,
            ClientId = authConfig?.ClientId ?? string.Empty,
            SignOutUrl = hashedAccountId == null ? new Uri($"{requestRoot}/signout/") : new Uri($"{requestRoot}/{hashedAccountId}/signout/"),
            ChangeEmailReturnUrl = new Uri($"{requestRoot}{requestPath}"),
            ChangePasswordReturnUrl = new Uri($"{requestRoot}{requestPath}")
        },
        new UserContext
        {
            User = html.ViewContext.HttpContext.User,
            HashedAccountId = html.ViewContext.RouteData.Values["accountId"]?.ToString() ?? string.Empty,
        },
        userName,
        isAbleToCallAuthenticatedServices,
        UseOriginalCode
        );

        headerModel.SelectMenu("Finance");

        if (html.ViewBag.HideNav is bool && html.ViewBag.HideNav)
        {
            headerModel.HideMenu();
        }

        return headerModel;
    }

    private static bool IsAbleToCallAuthenticatedServices(IHtmlHelper html)
    {
        bool isAuthenticated = false;
        if (html.ViewContext.HttpContext.User.Identity != null)
            isAuthenticated = html.ViewContext.HttpContext.User.Identity.IsAuthenticated;
        if (isAuthenticated && html.ViewContext != null && html.ViewContext.HttpContext != null && html.ViewContext.HttpContext.RequestServices != null)
        {
            ITokenService? tokenService = html.ViewContext.HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;
            if (tokenService != null && string.IsNullOrEmpty(tokenService.GetToken()))
            {
                return false;
            }
        }

        return isAuthenticated;
    }

    public static IFooterViewModel GetFooterViewModel(this IHtmlHelper html)
    {
        var externalLinks =
            (html.ViewContext.HttpContext.RequestServices.GetService(typeof(IOptions<ExternalLinksConfiguration>))
                as IOptions<ExternalLinksConfiguration>)?.Value;

#pragma warning disable CS8601 // Possible null reference assignment.
        return new FooterViewModel(new FooterConfiguration
        {
            ManageApprenticeshipsBaseUrl = externalLinks?.ManageApprenticeshipSiteUrl
        },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["accountId"]?.ToString()
            });
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    public static ICookieBannerViewModel GetCookieBannerViewModel(this IHtmlHelper html)
    {
        var externalLinks =
            (html.ViewContext.HttpContext.RequestServices.GetService(typeof(IOptions<ExternalLinksConfiguration>))
                as IOptions<ExternalLinksConfiguration>)?.Value;

#pragma warning disable CS8601 // Possible null reference assignment.
        return new CookieBannerViewModel(new CookieBannerConfiguration
        {
            ManageFamilyHubBaseUrl = externalLinks?.ManageApprenticeshipSiteUrl
        },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["accountId"]?.ToString()
            });
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    //the white space chars valid as separators between every two css class names
    static readonly char[] spaceChars = new char[] { ' ', '\t', '\r', '\n', '\f' };

    /// <summary> Adds or updates the specified css class to list of classes of this TagHelperOutput.</summary>
    public static void AddCssClass(this TagHelperOutput output, string newClass)
    {
        //get current class value:
        string? curClass = output.Attributes["class"]?.Value?.ToString(); //output.Attributes.FirstOrDefault(a => a.Name == "class")?.Value?.ToString();

        //check if newClass is null or equal to current class, nothing to do
        if (string.IsNullOrWhiteSpace(newClass) || string.Equals(curClass, newClass, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        //append newClass to end of curClass if curClass is not null and does not already contain newClass:
        if (!string.IsNullOrWhiteSpace(curClass)
            && curClass.Split(spaceChars, StringSplitOptions.RemoveEmptyEntries).Contains(newClass, StringComparer.OrdinalIgnoreCase)
            )
        {
            newClass = $"{curClass} {newClass}";
        }

        //set new css class value:
        output.Attributes.SetAttribute("class", newClass);

    }
}

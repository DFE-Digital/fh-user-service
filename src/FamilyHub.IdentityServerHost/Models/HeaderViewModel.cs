using FamilyHub.IdentityServerHost.Models.Configuration;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Models.Links;
using Microsoft.AspNetCore.Identity;

namespace FamilyHub.IdentityServerHost.Models;

public class HeaderViewModel : IHeaderViewModel
{
    const string GovUkHref = "https://www.gov.uk/";
    public IUserContext UserContext { get; private set; }
    public bool MenuIsHidden { get; private set; }
    public string SelectedMenu { get; private set; }

    public IReadOnlyList<Link> Links => _linkCollection.Links;

    public bool UseLegacyStyles { get; private set; }

    private readonly ILinkCollection _linkCollection;
    private readonly ILinkHelper _linkHelper;
    private readonly IUrlHelper _urlHelper;
    

    public HeaderViewModel(
        IHeaderConfiguration configuration,
        IUserContext userContext,
        string userName,
        ILinkCollection? linkCollection = null,
        ILinkHelper? linkHelper = null,
        IUrlHelper? urlHelper = null,
        bool useLegacyStyles = false)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        //ArgumentNullException.ThrowIfNull(userContext, nameof(userContext));

        //if (configuration == null) throw new ArgumentNullException("configuration");
        UserContext = userContext ?? throw new ArgumentNullException("userContext");

        _linkCollection = linkCollection ?? new LinkCollection();
        _linkHelper = linkHelper ?? new LinkHelper(_linkCollection);
        _urlHelper = urlHelper ?? new UrlHelper();
        UseLegacyStyles = useLegacyStyles;

        MenuIsHidden = false;
        SelectedMenu = "home";

        // Header links
        AddOrUpdateLink(new GovUk(GovUkHref, isLegacy: UseLegacyStyles));
        
        if (userContext != null && userContext.User != null && userContext.User.Identity != null)
        {
            
            if (userContext.User.Identity.IsAuthenticated)
            {
                AddOrUpdateLink(new HomeLink("/Index", UseLegacyStyles ? "" : "govuk-header__link govuk-header__link--service-name"));
                AddOrUpdateLink(new SignOutLink(userName, "/Identity/Account/Logout", UseLegacyStyles ? "" : "govuk-header__link govuk-header__link--service-name"));
            }
            else
            {
                AddOrUpdateLink(new SignInLink("/Identity/Account/Login", UseLegacyStyles ? "" : "govuk-header__link govuk-header__link--service-name"));
            }
        }



        
    }

    public void HideMenu()
    {
        MenuIsHidden = true;
    }

    public void SelectMenu(string menu)
    {
        SelectedMenu = menu;
    }

    public void AddOrUpdateLink<T>(T link) where T : Link
    {
        _linkCollection.AddOrUpdateLink(link);
    }

    public void RemoveLink<T>() where T : Link
    {
        _linkCollection.RemoveLink<T>();
    }

    public string RenderListItemLink<T>(bool isSelected = false, string @class = "") where T : Link
    {
        return _linkHelper.RenderListItemLink<T>(isSelected, @class);
    }

    public string RenderLink<T>(Func<string>? before = null, Func<string>? after = null, bool isSelected = false) where T : Link
    {
#pragma warning disable CS8604 // Possible null reference argument.
        return _linkHelper.RenderLink<T>(before, after, isSelected);
#pragma warning restore CS8604 // Possible null reference argument.
    }
}

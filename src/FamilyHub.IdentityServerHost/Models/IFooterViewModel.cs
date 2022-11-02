using FamilyHub.IdentityServerHost.Models;

namespace FamilyHubs.ServiceDirectoryAdminUi.Ui.Models;

public interface IFooterViewModel : ILinkCollection, ILinkHelper
{
    bool UseLegacyStyles { get; }
}

using System.Security.Principal;

namespace FamilyHub.IdentityServerHost.Models;

public interface IUserContext
{
    string HashedAccountId { get; set; }
    IPrincipal User { get; set; }
}

namespace FamilyHub.IdentityServerHost.Models.Configuration;

public interface IFooterConfiguration
{
    string ManageApprenticeshipsBaseUrl { get; set; }
    string AuthenticationAuthorityUrl { get; set; }
}

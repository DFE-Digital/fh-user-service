namespace FamilyHub.IdentityServerHost.Models.Configuration;

public class CookieBannerConfiguration : ICookieBannerConfiguration
{
    public string ManageFamilyHubBaseUrl { get; set; } = default!;
}

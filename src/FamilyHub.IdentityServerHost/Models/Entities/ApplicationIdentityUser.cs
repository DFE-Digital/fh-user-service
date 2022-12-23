using Microsoft.AspNetCore.Identity;

namespace FamilyHub.IdentityServerHost.Models.Entities;

public class ApplicationIdentityUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? FullName { get; set; }
}

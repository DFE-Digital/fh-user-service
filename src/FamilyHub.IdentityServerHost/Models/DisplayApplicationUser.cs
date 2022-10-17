namespace FamilyHub.IdentityServerHost.Models;

public class DisplayApplicationUser
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Roles { get; set; } = default!;
    public string? OrganisationId { get; set; } = default!;

}

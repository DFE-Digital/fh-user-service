namespace FamilyHub.IdentityServerHost.Models;

public class NewUser
{
    public string Role { get; set; } = default!;
    public string OrganisationId { get; set; } = default!;
    public string OrganisationName { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string EmailAddress { get; set; } = default!;
}

namespace FamilyHub.IdentityServerHost.Models.Entities;

public class UserOrganisation
{
    private UserOrganisation() { }
    public UserOrganisation(string id, string userId, string organisationId)
    {
        Id = id;
        UserId = userId;
        OrganisationId = organisationId;
    }

    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string OrganisationId { get; set; } = default!;
}

namespace FamilyHub.IdentityServerHost.Models.Entities;

public class OrganisationMapping
{
    private OrganisationMapping() { }
    public OrganisationMapping(string id, string parentOrganisationId, string organisationId)
    {
        Id = id;
        ParentOrganisationId = parentOrganisationId;
        OrganisationId = organisationId;
    }

    public string Id { get; set; } = default!;
    public string ParentOrganisationId { get; set; } = default!; // eg LA
    public string OrganisationId { get; set; } = default!; // eg Citizen Advice Bureau
}

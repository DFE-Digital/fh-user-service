namespace FamilyHub.IdentityServerHost.Models.Entities;

public class OrganisationType
{
    private OrganisationType() { }
    public OrganisationType(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    string Id { get; set; } = default!;
    string Name { get; set; } = default!;
    string Description { get; set; } = default!;
}

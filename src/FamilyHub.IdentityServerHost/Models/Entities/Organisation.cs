namespace FamilyHub.IdentityServerHost.Models.Entities;

public class Organisation
{
    private Organisation() { }

    public Organisation(
        string id,
        OrganisationType organisationType,
        string name = default!,
        string? description = default!,
        string? logo = default!,
        string? uri = default!,
        string? url = default!
    )
    {
        Id = id;
        OrganisationType = organisationType;
        Name = name ?? default!;
        Description = description ?? string.Empty;
        Logo = logo ?? string.Empty;
        Uri = uri ?? string.Empty;
        Url = url ?? string.Empty;
    }
    public string Id { get; set; } = string.Empty;
    public OrganisationType OrganisationType { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Logo { get; set; } = string.Empty;
    public string? Uri { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
}

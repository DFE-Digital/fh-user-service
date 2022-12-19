using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;

namespace FamilyHub.IdentityServerHost.Models;

public class NewOrganisation
{
    public OrganisationTypeDto? OrganisationTypeDto { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string OrganisationId { get; set; } = default!; //Admin District or Parent LA Id
    public string ParentName { get; set; } = default!; //Admin District or Parent LA Name
}

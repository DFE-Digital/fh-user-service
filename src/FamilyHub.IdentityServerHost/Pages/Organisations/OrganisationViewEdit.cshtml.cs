using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Pages.Organisations;

public class OrganisationViewEditModel : PageModel
{
    private readonly IApiService _apiService;

    public OrganisationViewEditModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public OrganisationModel Organisation { get; set; } = new OrganisationModel();
    public class OrganisationModel
    {
        public string Id { get; set; } = default!;

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string? Name { get; set; } = default!;

        public string? Description { get; set; }

        [DataType(DataType.Url)]
        public string? Logo { get; set; }

        public Uri? Uri { get; set; }

        [DataType(DataType.Url)]
        public string? Url { get; set; }
    }

    [BindProperty]
    public string OrgansationTypeId { get; set; } = default!;
    [BindProperty]
    public string AuthorityCode { get; set; } = default!;

    public async Task OnGet(string id, string organisationTypeId, string authorityCode)
    {
        
        OrgansationTypeId = organisationTypeId;
        AuthorityCode = authorityCode;

        if (!string.IsNullOrEmpty(id))
        {
            var apiOrganisation = await _apiService.GetOpenReferralOrganisationById(id);
            if (apiOrganisation != null)
            {
                Organisation.Id = apiOrganisation.Id;
                Organisation.Name = apiOrganisation.Name;
                Organisation.Description = apiOrganisation.Description;
                Organisation.Logo = apiOrganisation.Logo;
                Organisation.Url = apiOrganisation.Url;
            }
        }
    }
    public async Task<IActionResult> OnPost()
    {
        ModelState.Remove("Organisation.Id");

        if (!string.IsNullOrEmpty(Organisation.Url))
        {
            if (!Uri.IsWellFormedUriString(Organisation.Url, UriKind.Absolute))
            {
                ModelState.AddModelError("Organisation.Url", "Url is invalid");
            }
        }

        if (!string.IsNullOrEmpty(Organisation.Logo))
        {
            if (!Uri.IsWellFormedUriString(Organisation.Logo, UriKind.Absolute))
            {
                ModelState.AddModelError("Organisation.Logo", "Logo Url is invalid");
            }
        }

        if (!ModelState.IsValid)
            return Page();

        OrganisationTypeDto organisationType = default!;
        var organisationTypes = await _apiService.GetListOrganisationTypes();
        if (organisationTypes != null)
        {
            var orgType = organisationTypes.FirstOrDefault(x => x.Id == OrgansationTypeId);
            if (orgType != null)
                organisationType = orgType;
        }

        if (organisationType == null)
        {
            ModelState.AddModelError(String.Empty, "Unable to find orgaisation type");
            return Page();
        }
            

        OpenReferralOrganisationWithServicesDto openReferralOrganisationWithServicesDto = new(Organisation.Id, organisationType, Organisation.Name, Organisation.Description, Organisation.Logo, Organisation.Url, Organisation.Url, new List<OpenReferralServiceDto>());
        openReferralOrganisationWithServicesDto.AdministractiveDistrictCode = AuthorityCode;

        if (!string.IsNullOrEmpty(Organisation.Id))
        {
            await _apiService.UpdateOrganisation(openReferralOrganisationWithServicesDto);
        }
        else
        {
            await _apiService.CreateOrganisation(openReferralOrganisationWithServicesDto);
        }

        return RedirectToPage("ViewOrganisationList");

    }
}

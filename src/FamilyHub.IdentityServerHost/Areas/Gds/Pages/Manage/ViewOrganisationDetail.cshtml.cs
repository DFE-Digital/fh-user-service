using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class ViewOrganisationDetailModel : PageModel
{
    private readonly IApiService _apiService;

    public OpenReferralOrganisationWithServicesDto OpenReferralOrganisationDto { get; set; } = default!;
    [BindProperty]
    public string OrganisationId { get; set; } = default!;
    [BindProperty]
    public string? Name { get; set; } = default!;
    public string? LocalAuthority { get; set; } = default!;
    public bool ValidationValid { get; set; } = true;
    public ViewOrganisationDetailModel(IApiService apiService)
    {
        _apiService = apiService;
    }
    public async Task OnGet(string id)
    {
        OrganisationId = id;
        await InitPage();
    }

    public async Task OnPost()
    {
        if(string.IsNullOrEmpty(Name)) 
        {
            ModelState.AddModelError("Name", "Enter a name");
        }

        ValidationValid = ModelState.IsValid;
        if (!ModelState.IsValid)
        {
            await InitPage();
            return;
        }

        OpenReferralOrganisationDto = await _apiService.GetOpenReferralOrganisationById(OrganisationId);
        if (OpenReferralOrganisationDto != null && !string.IsNullOrEmpty(Name) && string.Compare(Name, OpenReferralOrganisationDto.Name) != 0)
        {
            OpenReferralOrganisationDto.Name= Name;
            await _apiService.UpdateOrganisation(OpenReferralOrganisationDto);
        }
        
        await InitPage();
    }

    private async Task InitPage()
    {
        if (OpenReferralOrganisationDto == null) 
        {
            OpenReferralOrganisationDto = await _apiService.GetOpenReferralOrganisationById(OrganisationId);
        }
        
        if (OpenReferralOrganisationDto != null)
        {
            OrganisationId = OpenReferralOrganisationDto.Id;
            Name = OpenReferralOrganisationDto.Name;

            if (OpenReferralOrganisationDto.OrganisationType.Name == "LA")
            {
                LocalAuthority = OpenReferralOrganisationDto.Name;
            }
            else
            {
                var allOrgs = await _apiService.GetListOpenReferralOrganisations();

                var las = allOrgs.FirstOrDefault(x => x.AdministractiveDistrictCode == OpenReferralOrganisationDto.AdministractiveDistrictCode);

                var la = allOrgs.FirstOrDefault(x => x.OrganisationType.Name == "LA" && x.AdministractiveDistrictCode == OpenReferralOrganisationDto.AdministractiveDistrictCode);
                if (la != null)
                {
                    LocalAuthority = la.Name;
                }
            }


        }
    }
}

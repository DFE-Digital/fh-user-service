using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;

public class CheckOrganisationDetailsModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly IApiService _apiService;
    public NewOrganisation? NewOrganisation { get; set; } = default!;

    public CheckOrganisationDetailsModel(IRedisCacheService redisCacheService, IApiService apiService)
    {
        _redisCacheService = redisCacheService;
        _apiService = apiService;
    }

    public void OnGet()
    {
        _redisCacheService.StoreCurrentPageName("/OrganisationWizard/CheckOrganisationDetails");
        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
    }
    public async Task<IActionResult> OnPost() 
    {
        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
        if (NewOrganisation != null) 
        {
            if (NewOrganisation?.OrganisationTypeDto?.Name == "LA")
            {
                OpenReferralOrganisationWithServicesDto openReferralOrganisationWithServicesDto = new(Guid.NewGuid().ToString(), NewOrganisation.OrganisationTypeDto, NewOrganisation.Name, NewOrganisation.Name, default!, default!, default!, new List<OpenReferralServiceDto>());
                openReferralOrganisationWithServicesDto.AdministractiveDistrictCode = NewOrganisation.OrganisationId;
                await _apiService.CreateOrganisation(openReferralOrganisationWithServicesDto);
                return RedirectToPage("/OrganisationWizard/Confirmation", new
                {
                    area = "Gds"
                });
            }
            else
            {
                OpenReferralOrganisationWithServicesDto parentLA = await _apiService.GetOpenReferralOrganisationById(NewOrganisation?.OrganisationId ?? string.Empty);
                if (parentLA != null) 
                {
                    OpenReferralOrganisationWithServicesDto openReferralOrganisationWithServicesDto = new(Guid.NewGuid().ToString(), NewOrganisation?.OrganisationTypeDto ?? new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"), NewOrganisation?.Name, NewOrganisation?.Name ?? string.Empty, default!, default!, default!, new List<OpenReferralServiceDto>());
                    openReferralOrganisationWithServicesDto.AdministractiveDistrictCode = parentLA.AdministractiveDistrictCode;
                    await _apiService.CreateOrganisation(openReferralOrganisationWithServicesDto);
                    return RedirectToPage("/OrganisationWizard/Confirmation", new
                    {
                        area = "Gds"
                    });
                }
                
            }
            
        }

        return Page();
    }
}

using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;

public class CheckOrganisationDetailsModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    public NewOrganisation? NewOrganisation { get; set; } = default!;

    public CheckOrganisationDetailsModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void OnGet()
    {
        _redisCacheService.StoreCurrentPageName("/OrganisationWizard/CheckOrganisationDetails");
        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
    }
    public void OnPost() 
    {
        //OpenReferralOrganisationWithServicesDto openReferralOrganisationWithServicesDto = new(Organisation.Id, organisationType, Organisation.Name, Organisation.Description, Organisation.Logo, Organisation.Url, Organisation.Url, new List<OpenReferralServiceDto>());
        //openReferralOrganisationWithServicesDto.AdministractiveDistrictCode = AuthorityCode;

        //if (!string.IsNullOrEmpty(Organisation.Id))
        //{
        //    await _apiService.UpdateOrganisation(openReferralOrganisationWithServicesDto);
        //}
        //else
        //{
        //    openReferralOrganisationWithServicesDto.Id = Guid.NewGuid().ToString();
        //    await _apiService.CreateOrganisation(openReferralOrganisationWithServicesDto);
        //}
    }
}

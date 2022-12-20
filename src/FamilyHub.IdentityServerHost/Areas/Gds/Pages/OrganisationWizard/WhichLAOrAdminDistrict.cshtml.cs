using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;

public class WhichLAOrAdminDistrictModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly IApiService _apiService;

    public List<SelectListItem> OrganisationSelectionList { get; set; } = new List<SelectListItem>();
    public NewOrganisation? NewOrganisation { get; set; } = default!;

    [Required]
    [BindProperty]
    public string OrganisationCode { get; set; } = default!;

    public WhichLAOrAdminDistrictModel(IRedisCacheService redisCacheService, IApiService apiService)
    {
        _redisCacheService = redisCacheService;
        _apiService = apiService;
    }

    public async Task OnGet()
    {
        _redisCacheService.StoreCurrentPageName("/OrganisationWizard/WhichLAOrAdminDistrict");
        await InitPage();
    }

    public async Task<IActionResult> OnPost()
    {
        await InitPage();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
        if (NewOrganisation != null)
        {
            NewOrganisation.OrganisationId = OrganisationCode;
            var item = OrganisationSelectionList.FirstOrDefault(x => x.Value == OrganisationCode);
            if (item != null)
            {
                NewOrganisation.ParentName = item.Text;
            }
        }
        
        _redisCacheService.StoreNewOrganisation(NewOrganisation);

        return RedirectToPage("/OrganisationWizard/OrganisationName", new
        {
            area = "Gds",
        });
    }

    private async Task InitPage()
    {
        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
        if (NewOrganisation?.OrganisationTypeDto?.Name == "LA")
        {
            var authorityList = StaticData.AuthorityCache.Select(x => new SelectListItem { Text = x.Value, Value = x.Key }).ToList();
            OrganisationSelectionList = authorityList.OrderBy(x => x.Text).ToList();
            OrganisationCode = authorityList[0].Value;
        }
        else
        {
            List<OpenReferralOrganisationDto>  laList = await _apiService.GetListOpenReferralOrganisations();
            laList = laList.Where(x => x.OrganisationType.Name == "LA").OrderBy(x => x.Name).ToList();
            OrganisationSelectionList = laList.Select(x => new SelectListItem { Text = x.Name, Value = x.Id }).ToList();
        }

        if (NewOrganisation != null && !string.IsNullOrEmpty(NewOrganisation.OrganisationId))
        {
            OrganisationCode = NewOrganisation.OrganisationId;
        }
         


    }
}

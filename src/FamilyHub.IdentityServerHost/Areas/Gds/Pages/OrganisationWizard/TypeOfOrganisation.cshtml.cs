using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;

public class TypeOfOrganisationModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly IApiService _apiService;

    public NewOrganisation? NewOrganisation { get; set; } = default!;
    public List<OrganisationTypeDto> OrganisationTypes { get; set; } = default!;

    [BindProperty]
    [Required]
    public string OrganisationTypeSelection { get; set; } = default!;

    public bool ValidationValid { get; set; } = true;

    public TypeOfOrganisationModel(IRedisCacheService redisCacheService, IApiService apiService)
    {
        _redisCacheService = redisCacheService;
        _apiService = apiService;
    }

    public async Task OnGet()
    {
        await InitPage();
        _redisCacheService.StoreCurrentPageName("/OrganisationWizard/TypeOfOrganisation");
        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
        if (NewOrganisation != null)
        {
            OrganisationTypeSelection = NewOrganisation?.OrganisationTypeDto?.Name ?? string.Empty;
        }
    }

    public async Task<IActionResult> OnPost()
    {
        await InitPage();

        ValidationValid = ModelState.IsValid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
        if (NewOrganisation == null)
        {
            NewOrganisation = new NewOrganisation();
        }
        NewOrganisation.OrganisationTypeDto = OrganisationTypes.FirstOrDefault(x => x.Name == OrganisationTypeSelection);

        
        _redisCacheService.StoreNewOrganisation(NewOrganisation);

        return RedirectToPage("/OrganisationWizard/WhichLAOrAdminDistrict", new
        {
            area = "Gds",
        });
    }

    private async Task InitPage()
    {
        OrganisationTypes = await _apiService.GetListOrganisationTypes();
        OrganisationTypes = OrganisationTypes.Where(x => x.Name != "FamilyHub").ToList();
    }
}

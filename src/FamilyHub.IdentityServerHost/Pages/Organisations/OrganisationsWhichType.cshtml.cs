using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Pages.Organisations;

public class OrganisationsWhichTypeModel : PageModel
{
    private readonly IApiService _apiService;

    [Required]
    [BindProperty(SupportsGet = true)]
    public string SelectedOrganisationType { get; set; } = default!;
    public List<SelectListItem> OrganisationTypeList { get; set; } = new List<SelectListItem>();

    [Required]
    [BindProperty(SupportsGet = true)]
    public string SelectedAuthority { get; set; } = default!;
    public List<SelectListItem> AuthorityList { get; set; } = new List<SelectListItem>();

    [BindProperty]
    public string OrganisationId { get; set; } = default!;

    public OrganisationsWhichTypeModel(IApiService apiService)
    {
        _apiService = apiService;
    }
    public async Task OnGet(string id)
    {
        OrganisationId = id;

        var organisationTypes = await _apiService.GetListOrganisationTypes();
        if(!User.IsInRole("DfEAdmin"))
        {
            organisationTypes = organisationTypes.Where(x => x.Name != "LA").ToList();
        }
        if (organisationTypes != null)
        {
            OrganisationTypeList = organisationTypes.Select(x => new SelectListItem { Text = x.Description, Value = x.Id }).ToList();
            SelectedOrganisationType = OrganisationTypeList[0].Value;
        }


        var organisations = await _apiService.GetListOpenReferralOrganisations();


        if (User.IsInRole("DfEAdmin"))
        {
            var authorityList = StaticData.AuthorityCache.Select(x => new SelectListItem { Text = x.Value, Value = x.Key }).ToList();
            AuthorityList = authorityList.OrderBy(x => x.Text).ToList();
            SelectedAuthority = authorityList[0].Value;

            if (!string.IsNullOrEmpty(OrganisationId))
            {
                OpenReferralOrganisationDto? organisation = organisations.FirstOrDefault(x => x.Id == OrganisationId);  //await _apiService.GetOpenReferralOrganisationById(OrganisationId);
                if (organisation != null)
                {
                    SelectedOrganisationType = organisation.OrganisationType.Id;
                }

                var authorityCode = await _apiService.GetAdminCodeByOrganisationId(OrganisationId);
                if (!string.IsNullOrEmpty(authorityCode))
                    SelectedAuthority = authorityCode;
            }
        }
        else
        {
            organisations = organisations.Where(x => x.OrganisationType.Name == "LA").ToList();
            var authorityList = organisations.Select(x => new SelectListItem { Text = x.Name, Value = x.AdministractiveDistrictCode }).ToList();
            AuthorityList = authorityList.OrderBy(x => x.Text).ToList();
            SelectedAuthority = authorityList[0].Value;
            if (!string.IsNullOrEmpty(OrganisationId))
            {
                var organisation = organisations.FirstOrDefault(x => x.Id == OrganisationId);
                if (organisation != null)
                {
                    SelectedAuthority = organisation.AdministractiveDistrictCode ?? authorityList[0].Value;
                }
            }
        }
            
        ModelState.Clear();
    }

    public IActionResult OnPost()
    {
        ModelState.Remove("OrganisationId");
            
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return RedirectToPage("/Organisations/OrganisationViewEdit", new
        {
            id = OrganisationId,
            organisationTypeId = SelectedOrganisationType,
            authorityCode = SelectedAuthority
        });
    }
}

using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class WhichOrganisationsModel : PageModel
{
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IApiService _apiService;
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    public List<SelectListItem> OrganisationSelectionList { get; set; } = new List<SelectListItem>();

    [BindProperty]
    public int OrganisationNumber { get; set; } = 1;

    [BindProperty]
    public List<string> OrganisationCode { get; set; } = new List<string>();

    [BindProperty]
    public List<string>? Organisations { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    [BindProperty]
    public bool AllOrganisationsSelected { get; set; } = true;

    [BindProperty]
    public bool NoDuplicateOrganisations { get; set; } = true;

    [BindProperty]
    public int OrganisationNotSelectedIndex { get; set; } = -1;

    [BindProperty]
    public List<string> OrganisationSelectedByField { get; set; } = default!;

    [BindProperty]
    public List<string> DuplicateFoundByField { get; set; } = default!;

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public WhichOrganisationsModel(IOrganisationRepository organisationRepository, IApiService apiService, UserManager<ApplicationIdentityUser> userManager)
    {
        _organisationRepository = organisationRepository;
        _apiService = apiService;
        _userManager = userManager;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        await InitPage();
        if(!User.IsInRole("DfEAdmin"))
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            var organisation = _organisationRepository.GetUserOrganisationIdByUserId(user.Id);
            if (!string.IsNullOrEmpty(organisation))
                OrganisationCode.Add(organisation);
        }      
    }

    private async Task InitPage()
    {
        var list = await _apiService.GetListOpenReferralOrganisations();
        OrganisationSelectionList = list.OrderBy(x => x.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id }).ToList();

        if (Organisations != null)
        {
            OrganisationCode = Organisations;
            OrganisationNumber = OrganisationCode.Count();
        }
        else
        {
            OrganisationCode.Add("Select organisation");
            OrganisationNumber = OrganisationCode.Count;
        }
    }

    public async Task OnPostAddAnotherOrganisation()
    {
        OrganisationCode.Add("Select organisation");
        OrganisationNumber = OrganisationCode.Count;
        await InitPage();
    }

    public async Task OnPostRemoveOrganisation(int id)
    {
        OrganisationCode.RemoveAt(id);
        OrganisationNumber = OrganisationCode.Count;
        await InitPage();
    }

    public async Task<IActionResult> OnPostNextPage()
    {
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            await InitPage();
            return Page();
        }

        if (Organisations != null)
        {
            for (int i = 0; i < Organisations.Count; i++)
            {
                if (Organisations[i] == null)
                {
                    OrganisationNotSelectedIndex = i;
                    OrganisationNumber = Organisations.Count;
                    ValidationValid = false;
                    AllOrganisationsSelected = false;
                    await InitPage();
                    return Page();
                }
            }

            for (int i = 0; i < Organisations.Count; i++)
            {
                for (int ii = 0; ii < Organisations.Count; ii++)
                {
                    if (Organisations[i] == Organisations[ii] && i != ii)
                    {
                        OrganisationNumber = Organisations.Count;
                        ValidationValid = false;
                        NoDuplicateOrganisations = false;
                        OrganisationNotSelectedIndex = ii;
                        await InitPage();
                        return Page();
                    }
                }
            }
        }

        var selected = string.Join(',', OrganisationCode ?? new List<string>());

        return RedirectToPage("./Register", new { returnUrl = ReturnUrl, organisations = string.Join(',', OrganisationCode ?? new List<string>()) });
    }
}

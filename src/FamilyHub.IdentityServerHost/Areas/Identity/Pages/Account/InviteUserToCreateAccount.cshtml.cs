using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class InviteUserToCreateAccountModel : PageModel
{
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IApiService _apiService;
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    public List<SelectListItem> OrganisationSelectionList { get; set; } = new List<SelectListItem>();

    public List<IdentityRole> AvailableRoles { get; set; } = new List<IdentityRole>(); // default(List<IdentityRole>);

    [BindProperty]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [BindProperty]
    public string RoleSelection { get; set; } = default!;

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

    public InviteUserToCreateAccountModel(IOrganisationRepository organisationRepository, IApiService apiService, UserManager<ApplicationIdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailSender emailSender)
    {
        _organisationRepository = organisationRepository;
        _apiService = apiService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _emailSender = emailSender;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        await InitPage();
        
    }

    private async Task InitPage()
    {
        
        if (User.IsInRole("DfEAdmin"))
            AvailableRoles = _roleManager.Roles.OrderBy(x => x.Name).ToList();
        else if (User.IsInRole("LAAdmin"))
            AvailableRoles = _roleManager.Roles.Where(x => x.Name != "DfEAdmin").OrderBy(x => x.Name).ToList();
        else if (User.IsInRole("VCSAdmin"))
            AvailableRoles = _roleManager.Roles.Where(x => x.Name != "DfEAdmin" && x.Name != "LAAdmin").OrderBy(x => x.Name).ToList();

        var list = await _apiService.GetListOpenReferralOrganisations();
        if (User.IsInRole("VCSAdmin"))
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            var orgaisationIds = _organisationRepository.GetAllUserOrganisationsByUserId(user.Id);
            OrganisationSelectionList = list.Where(x => orgaisationIds.Contains(x.Id)).OrderBy(x => x.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id }).ToList();
        }
        else if (User.IsInRole("LAAdmin"))
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            var organisationId = _organisationRepository.GetUserOrganisationIdByUserId(user.Id);
            var organisation = list.FirstOrDefault(x => x.Id == organisationId);
            if (organisation != null)
            {
                OrganisationSelectionList = list.Where(x => x.AdministractiveDistrictCode == organisation.AdministractiveDistrictCode).OrderBy(x => x.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id }).ToList();
            }
        }
        else
        {
            OrganisationSelectionList = list.OrderBy(x => x.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id }).ToList();
        }
        
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

        if (User.IsInRole("DfEAdmin"))
        {
            OrganisationSelectionList.Insert(0, new SelectListItem() { Text = "No Organisation" });
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

        if (OrganisationCode.Contains("No Organisation"))
        {
            OrganisationCode.Remove("No Organisation");
        }
        
        var selected = string.Join(',', OrganisationCode ?? new List<string>());

        var code = CreateAccountInvitationModel.GetTokenString(_configuration.GetValue<string>("InvitationKey"), Email, selected, RoleSelection, DateTime.UtcNow.AddDays(1));

        var callbackUrl = Url.Page(
                    "/Account/RegisterUserFromInvitation",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

        ArgumentNullException.ThrowIfNull(callbackUrl, nameof(callbackUrl));

        await _emailSender.SendEmailAsync(
                    Email,
                    "Invitation to Create An Account",
                    $"Please click to register an account (This link will expire in 24 hours) <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


        return RedirectToPage("./InviteUserSuccessful", new { email = Email, returnUrl = ReturnUrl });
    }
}

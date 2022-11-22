using FamilyHub.IdentityServerHost.Models;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Models.Entities;
using static FamilyHub.IdentityServerHost.Pages.Organisations.OrganisationViewEditModel;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class ManageUsersModel : PageModel
{
    
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IApiService _apiService;


    [BindProperty]
    public int PageNumber { get; set; } = 1;
    [BindProperty]
    public int PageSize { get; set; } = 10;

    public int TotalPages { get; set; } = 1;

    [BindProperty]
    public string Search { get; set; } = default!;
    [BindProperty]
    public string SearchName { get; set; } = default!;
    [BindProperty]
    public string SearchEmail { get; set; } = default!;
    [BindProperty]
    public string SearchRoles { get; set; } = default!;
    [BindProperty]
    public string SearchOrganisationName { get; set; } = default!;
    [BindProperty]
    public string SearchLocalAuthority { get; set; } = default!;

    public string OrganisationCode { get; set; } = default!;

    public string ErrorMessage { get; set; } = default!;

    public PaginatedList<DisplayApplicationUser> Users { get; set; } = new PaginatedList<DisplayApplicationUser>();
    public string ReturnUrl { get; set; } = default!;

    public ManageUsersModel(UserManager<ApplicationIdentityUser> userManager, IEmailSender emailSender, IOrganisationRepository organisationRepository, IApiService apiService)
    {
        _userManager = userManager;       
        _emailSender = emailSender;
        _organisationRepository = organisationRepository;
        _apiService = apiService;
    }

    public async Task OnGet(string pageNumber)
    {
        if (!int.TryParse(pageNumber, out var page))
        {
            page = 1;
        }

        var totalPages = _userManager.Users.Count() / PageSize;
        if (page < 1)
        {
            PageNumber = 1;    
        }
        else if (page > totalPages)
        {
            PageNumber = totalPages;
        }
        else
        {
            PageNumber = page;
        }

        await GetPage();
    }

    public async Task OnPost()
    {
        await GetPage();
    }

    private async Task GetPage()
    {
        ReturnUrl ??= Url.Content("~/Identity/Account/ManageUsers");
        
        List<UserOrganisation> userOrganisations = _organisationRepository.GetUserOrganisations();
        List<OpenReferralOrganisationDto> organisations = await _apiService.GetListOpenReferralOrganisations();

        var users = _userManager.Users.OrderBy(x => x.UserName).ToList();
        List<DisplayApplicationUser> applicationUsers = new();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            string? organisationId = null;
            string? organisationName = null;
            string? localAuthority = null;
            var userOrganisation = userOrganisations.FirstOrDefault(x => x.UserId == user.Id);
            if (userOrganisation != null)
            {
                organisationId = userOrganisation.OrganisationId;
                var org = organisations.FirstOrDefault(x => x.Id == userOrganisation.OrganisationId);
                if (org != null)
                {
                    organisationName = org.Name;
                    var la = organisations.FirstOrDefault(x => x.OrganisationType.Name == "LA" && x.AdministractiveDistrictCode == org.AdministractiveDistrictCode);
                    if (la != null)
                    {
                        localAuthority = la.Name;
                    }
                }  
            }
               
            applicationUsers.Add(new DisplayApplicationUser()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = string.Join(", ", roles),
                OrganisationId = organisationId,
                OrganisationName = organisationName ?? string.Empty,
                LocalAuthority = localAuthority ?? string.Empty
            });
        }

        //filter depending on user
        //Only show people in their own organisation
        if (!User.IsInRole("DfEAdmin"))
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = applicationUsers.FirstOrDefault(x => x.Email == userEmail);
            if (currentUser != null)
            {
                applicationUsers = applicationUsers.Where(x => x.Roles.Contains("DfEAdmin") == false).ToList();
                applicationUsers = applicationUsers.Where(x => x.OrganisationId == currentUser.OrganisationId || string.IsNullOrEmpty(x.OrganisationId)).ToList();
                var organisation = _organisationRepository.GetUserOrganisationIdByUserId(currentUser.Id);
                if (!string.IsNullOrEmpty(organisation))
                    OrganisationCode = organisation;
            }  
        }

        List<DisplayApplicationUser> pagelist;
        if (!string.IsNullOrEmpty(Search) ||
            !string.IsNullOrEmpty(SearchName) ||
            !string.IsNullOrEmpty(SearchEmail) ||
            !string.IsNullOrEmpty(SearchRoles) ||
            !string.IsNullOrEmpty(SearchOrganisationName) ||
            !string.IsNullOrEmpty(SearchLocalAuthority))
        {
            IEnumerable<DisplayApplicationUser> allPages = applicationUsers;
            if (!string.IsNullOrEmpty(Search))
            {
                allPages = allPages.Where(x => x.UserName.Contains(Search) || x.Email.Contains(Search) || x.Roles.Contains(Search) || x.OrganisationName.Contains(Search) || x.LocalAuthority.Contains(Search));
            }

            if (!string.IsNullOrEmpty(SearchName))
            {
                allPages = allPages.Where(x => x.UserName.Contains(SearchName));
            }

            if (!string.IsNullOrEmpty(SearchEmail))
            {
                allPages = allPages.Where(x => x.Email.Contains(SearchEmail));
            }

            if (!string.IsNullOrEmpty(SearchRoles))
            {
                allPages = allPages.Where(x => x.Roles.Contains(SearchRoles));
            }

            if (!string.IsNullOrEmpty(SearchOrganisationName))
            {
                allPages = allPages.Where(x => x.OrganisationName.Contains(SearchOrganisationName));
            }

            if (!string.IsNullOrEmpty(SearchLocalAuthority))
            {
                allPages = allPages.Where(x => x.LocalAuthority.Contains(SearchLocalAuthority));
            }

            pagelist = allPages.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            TotalPages = (int)Math.Ceiling((double)allPages.Count() / (double)PageSize);
            Users = new PaginatedList<DisplayApplicationUser>(pagelist, allPages.Count(), PageNumber, PageSize);
        }
        else
        {
            pagelist = applicationUsers.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            TotalPages = (int)Math.Ceiling((double)applicationUsers.Count / (double)PageSize);
            Users = new PaginatedList<DisplayApplicationUser>(pagelist, pagelist.Count, PageNumber, PageSize);
        }
    }

    public async Task<IActionResult> OnPostResetPassword(string id)
    {
        var user = await _userManager.FindByEmailAsync(id);
        if (user == null)
            return Page();

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { area = "Identity", code },
            protocol: Request.Scheme);

        try
        {
            await _emailSender.SendEmailAsync(
            user.Email,
            "Reset Password",
            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? string.Empty)}'>clicking here</a>.");
        }
        catch
        {
            ErrorMessage = "Unable to send email to reset password";
            return Page();
        }

        

        return RedirectToPage("./ForgotPasswordConfirmation");
    }

    public IActionResult OnPostDeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            return Page();

        return RedirectToPage($"./DeleteUser", new { id });
    }
}

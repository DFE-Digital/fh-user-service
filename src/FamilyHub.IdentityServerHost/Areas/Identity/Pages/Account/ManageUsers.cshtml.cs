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

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class ManageUsersModel : PageModel
{
    
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IOrganisationRepository _organisationRepository;
    

    [BindProperty]
    public int PageNumber { get; set; } = 1;
    [BindProperty]
    public int PageSize { get; set; } = 10;

    public int TotalPages { get; set; } = 1;

    [BindProperty]
    public string Search { get; set; } = default!;

    public string OrganisationCode { get; set; } = default!;

    public PaginatedList<DisplayApplicationUser> Users { get; set; } = new PaginatedList<DisplayApplicationUser>();
    public string ReturnUrl { get; set; } = default!;

    public ManageUsersModel(UserManager<ApplicationIdentityUser> userManager, IEmailSender emailSender, IOrganisationRepository organisationRepository)
    {
        _userManager = userManager;       
        _emailSender = emailSender;
        _organisationRepository = organisationRepository;
        
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

        var users = _userManager.Users.OrderBy(x => x.UserName).ToList();
        List<DisplayApplicationUser> applicationUsers = new();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            string? organisationId = null;
            var userOrganisation = userOrganisations.FirstOrDefault(x => x.UserId == user.Id);
            if (userOrganisation != null)
                organisationId = userOrganisation.OrganisationId;

            applicationUsers.Add(new DisplayApplicationUser()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = string.Join(", ", roles),
                OrganisationId = organisationId
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
        if (!string.IsNullOrEmpty(Search))
        {
            var allPages = applicationUsers.Where(x => x.UserName.Contains(Search) || x.Email.Contains(Search) || x.Roles.Contains(Search));
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

        await _emailSender.SendEmailAsync(
            user.Email,
            "Reset Password",
            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? string.Empty)}'>clicking here</a>.");

        return RedirectToPage("./ForgotPasswordConfirmation");
    }

    public IActionResult OnPostDeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            return Page();

        return RedirectToPage($"./DeleteUser", new { id });
    }
}

using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHub.IdentityServerHost.Models;
using FamilyHubs.SharedKernel;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using System.Security.Claims;
using FamilyHub.IdentityServerHost.Helpers;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class ViewUsersModel : PageModel
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IApiService _apiService;

    public ViewUsersModel(UserManager<ApplicationIdentityUser> userManager, IEmailSender emailSender, IOrganisationRepository organisationRepository, IApiService apiService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _organisationRepository = organisationRepository;
        _apiService = apiService;
    }

    [BindProperty]
    public string DfEAdmin { get; set; } = default!;

    [BindProperty]
    public string LAAdmin { get; set; } = default!;

    [BindProperty]
    public string VCSAdmin { get; set; } = default!;

    [BindProperty]
    public string Professional { get; set; } = default!;

    [BindProperty]
    public int PageNumber { get; set; } = 1;
    [BindProperty]
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;
    public string OrganisationCode { get; set; } = default!;
    public string ErrorMessage { get; set; } = default!;
    public PaginatedList<DisplayApplicationUser> Users { get; set; } = new PaginatedList<DisplayApplicationUser>();
    public string ReturnUrl { get; set; } = default!;

    public async Task OnGet(string pageNumber)
    {
        if (!int.TryParse(pageNumber, out var page))
        {
            page = 1;
        }

        var totalPages = (int)Math.Ceiling( (float)_userManager.Users.Count() / (float)PageSize);
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

    public async Task OnGetClearFilters()
    {
        DfEAdmin = default!;
        LAAdmin = default!;
        VCSAdmin = default!;
        Professional = default!;

        await GetPage();
    }

    

    public async Task OnPost()
    {
        await GetPage();
    }

    private async Task GetPage()
    {
        ReturnUrl ??= Url.Content("~/Gds/Manage/ViewUsers");

        List<UserOrganisation> userOrganisations = _organisationRepository.GetUserOrganisations();
        List<OpenReferralOrganisationDto> organisations = await _apiService.GetListOpenReferralOrganisations();

        var users = _userManager.Users.OrderBy(x => x.UserName).ToList();
        List<DisplayApplicationUser> applicationUsers = new();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            List<string> roleNames = new List<string>();
            foreach (var role in roles) 
            {
                roleNames.Add(RoleHelper.GetRoleFullName(role));
            }
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
                FullRoleNames = string.Join(", ", roleNames),
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

        List<string> Keys= new List<string>();
        if (!string.IsNullOrEmpty(DfEAdmin)) 
        {
            Keys.Add("DfEAdmin");
        }
        if (!string.IsNullOrEmpty(LAAdmin))
        {
            Keys.Add("LAAdmin");
        }
        if (!string.IsNullOrEmpty(VCSAdmin))
        {
            Keys.Add("VCSAdmin");
        }   
        if (!string.IsNullOrEmpty(Professional))
        {
            Keys.Add("Professional");
        }

        if (Keys.Any())
        {
            IEnumerable<DisplayApplicationUser> allPages = applicationUsers.Where(x => Keys.Contains(x.Roles));

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

}

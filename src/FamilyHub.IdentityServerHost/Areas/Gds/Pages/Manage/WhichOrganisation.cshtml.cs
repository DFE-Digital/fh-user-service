using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class WhichOrganisationModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IApiService _apiService;
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public NewUser? NewUser { get; set; } = default!;

    public List<SelectListItem> OrganisationSelectionList { get; set; } = new List<SelectListItem>();

    [Required]
    [BindProperty]
    public string OrganisationCode { get; set; } = default!;

    public WhichOrganisationModel(IRedisCacheService redisCacheService, IOrganisationRepository organisationRepository, IApiService apiService, UserManager<ApplicationIdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _redisCacheService = redisCacheService;
        _organisationRepository = organisationRepository;
        _apiService = apiService;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task OnGet()
    {
        _redisCacheService.StoreCurrentPageName("/Manage/WhichOrganisation");
        NewUser = _redisCacheService.RetrieveNewUser();
        if(NewUser != null) 
        {
            OrganisationCode = NewUser.OrganisationId;
        }
        await Init();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) 
        {
            await Init();
            return Page();
        }

        NewUser = _redisCacheService.RetrieveNewUser();
        if (NewUser != null)
        {
            NewUser.OrganisationId = OrganisationCode;
            var list = await _apiService.GetListOpenReferralOrganisations();
            if(list != null) 
            {
                var org = list.FirstOrDefault(x => x.Id == OrganisationCode);
                if (org != null && org.Name != null) 
                {
                    NewUser.OrganisationName = org.Name;
                }
            }
        }
        _redisCacheService.StoreNewUser(NewUser);

        return RedirectToPage("/Manage/WhatIsUsername", new
        {
            area = "Gds",
        });
    }

    private async Task Init()
    {
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
    }
}

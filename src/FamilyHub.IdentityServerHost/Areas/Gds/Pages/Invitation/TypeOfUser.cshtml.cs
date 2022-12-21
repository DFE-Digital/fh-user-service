using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation;

public class TypeOfUserModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly RoleManager<IdentityRole> _roleManager;

    public NewUser? NewUser { get; set; } = default!;
    public List<IdentityRole> AvailableRoles { get; set; } = new List<IdentityRole>();

    [BindProperty]
    [Required]
    public string RoleSelection { get; set; } = default!;

    public TypeOfUserModel(IRedisCacheService redisCacheService, RoleManager<IdentityRole> roleManager)
    { 
        _roleManager = roleManager;
        _redisCacheService = redisCacheService;
    }
    public void OnGet()
    {
        _redisCacheService.StoreCurrentPageName("/Invitation/TypeOfUser");
        NewUser = _redisCacheService.RetrieveNewUser();
        if (NewUser != null) 
        {
            RoleSelection = NewUser.Role;
        }
        
        InitPage();
    }

    public IActionResult OnPost() 
    { 
        
        if (string.IsNullOrEmpty(RoleSelection))
        {
            ModelState.AddModelError("RoleSelection", "Please select a role");
        }

        if (!ModelState.IsValid) 
        {
            InitPage();
            return Page();
        }

        NewUser = _redisCacheService.RetrieveNewUser();
        if (NewUser == null) 
        { 
            NewUser = new NewUser();
        }
        NewUser.Role = RoleSelection;
        _redisCacheService.StoreNewUser(NewUser);

        if (RoleSelection != "DfEAdmin")
        {
            return RedirectToPage("/Invitation/WhichOrganisation", new
            {
                area = "Gds",
            });
        }

        return RedirectToPage("/Invitation/WhatIsUsername", new
        {
            area = "Gds",
        });
    }

    private void InitPage()
    {
        if (User.IsInRole("DfEAdmin"))
            AvailableRoles = _roleManager.Roles.OrderBy(x => x.Name).ToList();
        else if (User.IsInRole("LAAdmin"))
            AvailableRoles = _roleManager.Roles.Where(x => x.Name != "DfEAdmin").OrderBy(x => x.Name).ToList();
        else if (User.IsInRole("VCSAdmin"))
            AvailableRoles = _roleManager.Roles.Where(x => x.Name != "DfEAdmin" && x.Name != "LAAdmin").OrderBy(x => x.Name).ToList();

    }
}

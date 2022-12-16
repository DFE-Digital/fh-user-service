using FamilyHub.IdentityServerHost.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class TypeOfUserModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public List<IdentityRole> AvailableRoles { get; set; } = new List<IdentityRole>();

    [BindProperty]
    [Required]
    public string RoleSelection { get; set; } = default!;

    public TypeOfUserModel(RoleManager<IdentityRole> roleManager)
    { 
        _roleManager = roleManager;
    }
    public void OnGet()
    {
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

        return RedirectToPage("/Manage/WhatIsUsername", new
        {
            area = "Gds",
            role = RoleSelection
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

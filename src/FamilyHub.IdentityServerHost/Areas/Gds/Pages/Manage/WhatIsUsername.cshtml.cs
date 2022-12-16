using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class WhatIsUsernameModel : PageModel
{
    public string Role { get; private set; } = default!;

    [Required]
    [BindProperty]
    public string FullName { get; set; } = default!;
    public void OnGet(string role)
    {
        Role = role;
    }

    public IActionResult OnPost(string role)
    {
        Role = role;

        if (!ModelState.IsValid) 
        {
            return Page();
        }

        return RedirectToPage("/Manage/WhatIsEmailAddress", new
        {
            area = "Gds",
            role = role,
            fullName = FullName
        });
    }
}

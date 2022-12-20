using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation;

public class WhatIsUsernameModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    public NewUser? NewUser { get; set; } = default!;

    [Required]
    [BindProperty]
    public string FullName { get; set; } = default!;

    public WhatIsUsernameModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void OnGet()
    {
        _redisCacheService.StoreCurrentPageName("/Invitation/WhatIsUsername");
        NewUser = _redisCacheService.RetrieveNewUser();
        if (NewUser != null)
        {
            FullName = NewUser.FullName;
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) 
        {
            return Page();
        }

        NewUser = _redisCacheService.RetrieveNewUser();
        if (NewUser != null)
        {
            NewUser.FullName = FullName;
        }
        _redisCacheService.StoreNewUser(NewUser);

        return RedirectToPage("/Invitation/WhatIsEmailAddress", new
        {
            area = "Gds",
        });
    }
}

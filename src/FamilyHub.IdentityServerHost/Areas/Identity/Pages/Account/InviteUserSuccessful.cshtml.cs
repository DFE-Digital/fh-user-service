using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class InviteUserSuccessfulModel : PageModel
{
    [BindProperty]
    public string? ReturnUrl { get; set; }

    public string Email { get; set; } = default!;

    public void OnGet(string email, string? returnUrl = null)
    {
        Email = email;
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/Index");
    }
}

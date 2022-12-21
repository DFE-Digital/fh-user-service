using FamilyHub.IdentityServerHost.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;

public class ViewPersonalDetailsModel : PageModel
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;

    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;

    public ViewPersonalDetailsModel(UserManager<ApplicationIdentityUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        Username = await _userManager.GetUserNameAsync(user);
        Email = await _userManager.GetEmailAsync(user);

        return Page();
    }
}

using FamilyHub.IdentityServerHost.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;

public class ChangeNameModel : PageModel
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;

    [BindProperty]
    [Required]
    public string Username { get; set; } = default!;

    public string StatusMessage { get; set; } = default!;

    public ChangeNameModel(UserManager<ApplicationIdentityUser> userManager)
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

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid) 
        {
            return Page();
        }

        var setUserNameResult = await _userManager.SetUserNameAsync(user, Username);
        if (!setUserNameResult.Succeeded)
        {
            StatusMessage = "Error changing user name.";
            return Page();
        }

        return RedirectToPage("/MyAccount/ConfirmNameChanged", new
        {
            area = "Gds",
        });
    }
}

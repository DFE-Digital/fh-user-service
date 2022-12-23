using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;

public class ChangeNameModel : PageModel
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IApplicationDbContext _applicationDbContext;

    [BindProperty]
    [Required]
    public string Username { get; set; } = default!;

    public string StatusMessage { get; set; } = default!;
    public bool ValidationValid { get; set; } = true;

    public ChangeNameModel(UserManager<ApplicationIdentityUser> userManager, IApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
    }
    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        Username = _applicationDbContext.GetFullName(user.Email) ?? string.Empty;
        if (string.IsNullOrEmpty(Username)) 
        {
            Username = await _userManager.GetUserNameAsync(user);
        }   
            

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        ValidationValid = ModelState.IsValid;
        if (!ModelState.IsValid) 
        {
            return Page();
        }

        if (!await _applicationDbContext.SetFullNameAsync(user.Email, Username))
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

using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.MyAccount;

public class ViewPersonalDetailsModel : PageModel
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IApplicationDbContext _applicationDbContext;

    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;

    public ViewPersonalDetailsModel(UserManager<ApplicationIdentityUser> userManager, IApplicationDbContext applicationDbContext)
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
        
        Email = await _userManager.GetEmailAsync(user);

        return Page();
    }
}

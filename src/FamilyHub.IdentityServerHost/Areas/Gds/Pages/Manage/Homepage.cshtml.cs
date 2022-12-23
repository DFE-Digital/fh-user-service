using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class HomepageModel : PageModel
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IApplicationDbContext _applicationDbContext;

    public string UserName { get; set; } = default!;

    public HomepageModel(UserManager<ApplicationIdentityUser> userManager, IApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
    }
    public async Task OnGet()
    {
        UserName = _userManager.GetUserName(User);
        string? userName = _applicationDbContext.GetFullNameAsync(UserName);
        if (userName != null) 
        { 
            UserName = userName;
        } 
    }
}

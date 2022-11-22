using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenService _tokenService;

        public IndexModel(ITokenService tokenService, ILogger<IndexModel> logger)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(_tokenService.GetToken()))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            return Page();
        }
    }
}
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenService _tokenService;

        public bool UseOriginalCode { get; set; } = false;

        public IndexModel(IConfiguration configuration, ITokenService tokenService, ILogger<IndexModel> logger)
        {
            _logger = logger;
            _tokenService = tokenService;
            UseOriginalCode = configuration.GetValue<bool>("UseOriginalCode");
        }

        public IActionResult OnGet()
        {
            if (UseOriginalCode)
            {
                if (string.IsNullOrEmpty(_tokenService.GetToken()))
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_tokenService.GetToken()))
                {
                    return RedirectToPage("/Manage/Homepage", new { area = "Gds" });
                }
            }
            
            
            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(_tokenService.GetToken()))
            {
                return RedirectToPage("/Account/Login", new { area = "Gds" });
            }

            return Page();
        }
    }
}
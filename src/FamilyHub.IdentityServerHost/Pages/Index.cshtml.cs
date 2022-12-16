using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenService _tokenService;

        public bool UseOrginalCode { get; set; } = false;

        public IndexModel(ITokenService tokenService, ILogger<IndexModel> logger)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        public IActionResult OnGet()
        {
            if (UseOrginalCode)
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
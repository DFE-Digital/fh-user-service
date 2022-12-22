using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation
{
    public class WhatIsEmailAddressModel : PageModel
    {
        private readonly IRedisCacheService _redisCacheService;
        public NewUser? NewUser { get; set; } = default!;

        public WhatIsEmailAddressModel(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        [Required(ErrorMessage = "Enter an email address")]
        [BindProperty]
        [EmailAddress]
        public string EmailAddress { get; set; } = default!;

        public bool ValidationValid { get; set; } = true;
        public void OnGet()
        {
            _redisCacheService.StoreCurrentPageName("/Invitation/WhatIsEmailAddress");
            NewUser = _redisCacheService.RetrieveNewUser();
            if (NewUser != null)
            {
                EmailAddress = NewUser.EmailAddress;
            }
        }

        public IActionResult OnPost()
        {
            ValidationValid = ModelState.IsValid;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewUser = _redisCacheService.RetrieveNewUser();
            if (NewUser != null)
            {
                NewUser.EmailAddress = EmailAddress;
            }
            _redisCacheService.StoreNewUser(NewUser);

            return RedirectToPage("/Invitation/CheckAccountDetails", new
            {
                area = "Gds",
            });
        }
    }
}

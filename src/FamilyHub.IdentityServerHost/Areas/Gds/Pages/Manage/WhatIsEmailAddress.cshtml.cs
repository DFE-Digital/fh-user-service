using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage
{
    public class WhatIsEmailAddressModel : PageModel
    {
        private readonly IRedisCacheService _redisCacheService;
        public NewUser? NewUser { get; set; } = default!;

        public WhatIsEmailAddressModel(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        [Required]
        [BindProperty]
        public string EmailAddress { get; set; } = default!;
        public void OnGet()
        {
            _redisCacheService.StoreCurrentPageName("WhatIsEmailAddress");
            NewUser = _redisCacheService.RetrieveNewUser();
            if (NewUser != null)
            {
                EmailAddress = NewUser.EmailAddress;
            }
        }

        public IActionResult OnPost()
        {
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

            return RedirectToPage("/Manage/CheckAccountDetails", new
            {
                area = "Gds",
            });
        }
    }
}

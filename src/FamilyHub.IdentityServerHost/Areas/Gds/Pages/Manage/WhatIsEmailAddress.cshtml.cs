using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage
{
    public class WhatIsEmailAddressModel : PageModel
    {
        public string Role { get; private set; } = default!;

        public string FullName { get; private set; } = default!;

        [Required]
        [BindProperty]
        public string EmailAddress { get; set; } = default!;
        public void OnGet(string role, string fullName)
        {
            Role = role;
            FullName = fullName;
        }

        public IActionResult OnPost(string role, string fullName)
        {
            Role = role;
            FullName = fullName;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            return RedirectToPage("/Manage/CheckAccountDetails", new
            {
                area = "Gds",
                role = role,
                fullName = FullName,
                emailAddress = EmailAddress
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation
{
    public class ConfirmUserAccountSetUpModel : PageModel
    {
        public string UserName { get; set; } = default!;
        public void OnGet(string username)
        {
            UserName = username;
        }
    }
}

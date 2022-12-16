using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class CheckAccountDetailsModel : PageModel
{
    public string Role { get; private set; } = default!;
    public string RoleName { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string EmailAddress { get; set; } = default!;
    public void OnGet(string role, string fullName, string emailAddress)
    {
        Role = role;
        FullName = fullName;
        EmailAddress = emailAddress;
        GetRoleFullName();

    }

    public void OnPost(string role, string fullName, string emailAddress)
    {

    }

    private void GetRoleFullName()
    {
        switch(Role)
        {
            case "DfEAdmin":
                RoleName = "Department for Education administrator";
                break;
            case "LAAdmin":
                RoleName = "Local authority administrator";
                break;
            case "VCSAdmin":
                RoleName = "Voluntary community organisation administrator";
                break;
            case "Professional":
                RoleName = "Department for Education administrator";
                break;
        }             
    }
}

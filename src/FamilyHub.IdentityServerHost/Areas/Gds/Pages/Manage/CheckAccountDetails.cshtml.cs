using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Encodings.Web;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class CheckAccountDetailsModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    public string RoleName { get; set; } = default!;

    public NewUser? NewUser { get; set; } = default!;

    public CheckAccountDetailsModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void OnGet()
    {
        _redisCacheService.StoreCurrentPageName("CheckAccountDetails");
        NewUser = _redisCacheService.RetrieveNewUser();
        GetRoleFullName();

    }

    public IActionResult OnPost()
    {
        //ToDo Invite
        //var code = CreateAccountInvitationModel.GetTokenString(_configuration.GetValue<string>("InvitationKey"), Email, selected, RoleSelection, DateTime.UtcNow.AddDays(1));

        //var callbackUrl = Url.Page(
        //            "/Account/RegisterUserFromInvitation",
        //            pageHandler: null,
        //            values: new { area = "Identity", code },
        //            protocol: Request.Scheme);

        //ArgumentNullException.ThrowIfNull(callbackUrl, nameof(callbackUrl));

        //await _emailSender.SendEmailAsync(
        //            Email,
        //            "Invitation to Create An Account",
        //            $"Please click to register an account (This link will expire in 24 hours) <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        _redisCacheService.ResetLastPageName();
        _redisCacheService.ResetNewUser();

        return RedirectToPage("/Manage/Confirmation", new
        {
            area = "Gds"
        });
    }

    private void GetRoleFullName()
    {
        switch(NewUser?.Role)
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

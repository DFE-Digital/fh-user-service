using FamilyHub.IdentityServerHost.Helpers;
using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Encodings.Web;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Invitation;

public class CheckAccountDetailsModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    public string LastPage { get; set; } = default!;
    public string RoleName { get; set; } = default!;

    public NewUser? NewUser { get; set; } = default!;

    public CheckAccountDetailsModel(IRedisCacheService redisCacheService, IConfiguration configuration, IEmailSender emailSender)
    {
        _redisCacheService = redisCacheService;
        _configuration = configuration;
        _emailSender = emailSender;
    }

    public void OnGet()
    {
        LastPage = _redisCacheService.RetrieveLastPageName();
        _redisCacheService.StoreCurrentPageName("/Invitation/CheckAccountDetails");
        NewUser = _redisCacheService.RetrieveNewUser();
        RoleName = RoleHelper.GetRoleFullName(NewUser?.Role ?? string.Empty);
    }

    public async Task<IActionResult> OnPost()
    {
        NewUser = _redisCacheService.RetrieveNewUser();
        if (NewUser != null) 
        {
            var code = CreateAccountInvitationModel.GetTokenString(_configuration.GetValue<string>("InvitationKey"),NewUser.FullName, NewUser.EmailAddress, NewUser.OrganisationId, NewUser.Role, DateTime.UtcNow.AddDays(1));

            var callbackUrl = Url.Page(
                        "/Account/RegisterUserFromInvitation",
                        pageHandler: null,
                        values: new { area = "Identity", code },
                        protocol: Request.Scheme);

            ArgumentNullException.ThrowIfNull(callbackUrl, nameof(callbackUrl));

            await _emailSender.SendEmailAsync(
                        NewUser.EmailAddress,
                        "Invitation to Create An Account",
                        $"Please click to register an account (This link will expire in 24 hours) <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            _redisCacheService.ResetLastPageName();
            _redisCacheService.ResetNewUser();

            return RedirectToPage("/Invitation/Confirmation", new
            {
                area = "Gds"
            });
        }

        return Page();
    }
}

using FamilyHub.IdentityServerHost.Helpers;
using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class ViewUserModel : PageModel
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IApiService _apiService;

    public DisplayApplicationUser DisplayApplicationUser { get; set; } = default!;

    public ViewUserModel(UserManager<ApplicationIdentityUser> userManager, IEmailSender emailSender, IOrganisationRepository organisationRepository, IApiService apiService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _organisationRepository = organisationRepository;
        _apiService = apiService;
    }

    public async Task OnGet(string emailAddress)
    {
        await InitPage(emailAddress);
    }

    public async Task InitPage(string emailAddress)
    {
        List<UserOrganisation> userOrganisations = _organisationRepository.GetUserOrganisations();
        List<OpenReferralOrganisationDto> organisations = await _apiService.GetListOpenReferralOrganisations();
        var user = _userManager.Users.FirstOrDefault(x => x.Email == emailAddress);
        var roles = await _userManager.GetRolesAsync(user ?? new ApplicationIdentityUser());
        List<string> roleNames = new();
        foreach (var role in roles)
        {
            roleNames.Add(RoleHelper.GetRoleFullName(role));
        }
        string? organisationId = null;
        string? organisationName = null;
        string? localAuthority = null;
        var userOrganisation = userOrganisations.FirstOrDefault(x => x.UserId == user?.Id);
        if (userOrganisation != null)
        {
            organisationId = userOrganisation.OrganisationId;
            var org = organisations.FirstOrDefault(x => x.Id == userOrganisation.OrganisationId);
            if (org != null)
            {
                organisationName = org.Name;
                var la = organisations.FirstOrDefault(x => x.OrganisationType.Name == "LA" && x.AdministractiveDistrictCode == org.AdministractiveDistrictCode);
                if (la != null)
                {
                    localAuthority = la.Name;
                }
            }
        }



        DisplayApplicationUser = new DisplayApplicationUser()
        {
            Id = user?.Id ?? string.Empty,
            UserName = user?.UserName ?? string.Empty,
            Email = user?.Email ?? string.Empty,
            FullRoleNames = string.Join(", ", roleNames),
            Roles = string.Join(", ", roles),
            OrganisationId = organisationId,
            OrganisationName = organisationName ?? string.Empty,
            LocalAuthority = localAuthority ?? string.Empty
        };
    }

    public async Task<IActionResult> OnGetResetPassword(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);
        if (user == null)
        {
            await InitPage(emailAddress);
            return Page();
        }
            

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { area = "Identity", code },
            protocol: Request.Scheme);

        try
        {
            await _emailSender.SendEmailAsync(
            user.Email,
            "Reset Password",
            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? string.Empty)}'>clicking here</a>.");
        }
        catch
        {
            //ErrorMessage = "Unable to send email to reset password";
            await InitPage(emailAddress);
            return Page();
        }

        return RedirectToPage("/Account/ForgotPasswordConfirmation", new
        {
            area = "Identity",
        });

        
    }

    public async Task<IActionResult> OnGetDeleteUser(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);
        if (user == null)
        {
            await InitPage(emailAddress);
            return Page();
        }

        if (string.IsNullOrEmpty(user.Id))
            return Page();

        return RedirectToPage("/Account/DeleteUser", new
        {
            area = "Identity",
            id = user.Id
        });

        
    }
}

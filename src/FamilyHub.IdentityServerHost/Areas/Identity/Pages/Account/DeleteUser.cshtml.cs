using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class DeleteUserModel : PageModel
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly ILogger<DeleteUserModel> _logger;
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IConfiguration _configuration;

    public DeleteUserModel(
        UserManager<ApplicationIdentityUser> userManager,
        IOrganisationRepository organisationRepository,
        ILogger<DeleteUserModel> logger,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _logger = logger;
        _organisationRepository = organisationRepository;
        _configuration = configuration;
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }

    public string Username { get; set; } = default!;

    [BindProperty]
    public string UserId { get; set; } = default!;
    public async Task<IActionResult> OnGet(string id)
    {
        UserId = id;
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound($"Unable to load user.");
        }

        Username = user.UserName;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.FindByIdAsync(UserId);
        if (user == null)
        {
            return NotFound($"Unable to load user.");
        }

        if (!await _userManager.CheckPasswordAsync(user, Input.Password))
        {
            ModelState.AddModelError(string.Empty, "Incorrect password.");
            return Page();
        }

        await _organisationRepository.DeleteUserByUserIdAsync(user.Id);

        var result = await _userManager.DeleteAsync(user);
        var userId = await _userManager.GetUserIdAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error occurred deleting user.");
        }

        _logger.LogInformation($"User with ID '{UserId}' deleted by Admin.");

        if(_configuration.GetValue<bool>("UseOriginalCode"))
        {
            return Redirect("~/Identity/Account/ManageUsers");
        }
        else
        {
            return Redirect("~/Gds/Manage/ViewUsers");
        }
        
    }
}
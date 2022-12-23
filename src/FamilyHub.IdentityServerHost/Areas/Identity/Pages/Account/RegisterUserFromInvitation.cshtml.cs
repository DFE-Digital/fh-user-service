using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class RegisterUserFromInvitationModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly SignInManager<ApplicationIdentityUser> _signInManager;
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IUserStore<ApplicationIdentityUser> _userStore;
    private readonly IUserEmailStore<ApplicationIdentityUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IApiService _apiService;
    private readonly IMemoryCache _memoryCache;
    private readonly IApplicationDbContext _applicationDbContext;


    public bool LinkHasExpired = false;

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

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
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = default!;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = default!;

        public string OrganisationId { get; set; } = default!;
        public string Role { get; set; } = default!;

        public string Fullname { get; set; } = default!;
    }

    public RegisterUserFromInvitationModel(
        IConfiguration configuration,
        UserManager<ApplicationIdentityUser> userManager,
            IUserStore<ApplicationIdentityUser> userStore,
            SignInManager<ApplicationIdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IOrganisationRepository organisationRepository,
            IApiService apiService,
            IMemoryCache memoryCache,
            IApplicationDbContext applicationDbContext)
    {
        _configuration = configuration;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _roleManager = roleManager;
        _organisationRepository = organisationRepository;
        _apiService = apiService;
        _memoryCache = memoryCache;
        _applicationDbContext = applicationDbContext;
    }
    public IActionResult OnGet(string? code = null)
    {
        if (code == null)
        {
            return BadRequest("A code must be supplied to create a new user account.");
        }
        else
        {
            int index = code.LastIndexOf("'");
            if (index >= 0)
                code = code.Substring(0, index);

            var invitationModel = CreateAccountInvitationModel.GetCreateAccountInvitationModel(_configuration.GetValue<string>("InvitationKey"), code);

            if (invitationModel == null || DateTime.UtcNow > invitationModel.DateExpired)
            {
                LinkHasExpired = true;
                return Page();
            }

            Input.Email = invitationModel.EmailAddress;
            Input.OrganisationId = invitationModel.OrganisationId;
            Input.Role = invitationModel.Role;
            Input.Fullname = invitationModel.Username;

            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.Compare(Input.Role, "DfEAdmin", StringComparison.OrdinalIgnoreCase) == 0)
            ModelState.Remove("Input.OrganisationId");
        else
        {
            if (Input.OrganisationId == null)
                ModelState.AddModelError("", "You are not associated with an organisation.");
        }

        

        ModelState.Remove("Input.Role");

        if (_configuration.GetValue<bool>("UseOriginalCode"))
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await AddUserOrganisation(user);
                    await AddUserRoles(user);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = "/Account/RegisterUserFromInvitation" },
                        protocol: Request.Scheme);

                    ArgumentNullException.ThrowIfNull(callbackUrl, nameof(callbackUrl));

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = "/Account/RegisterUserFromInvitation" });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect("~/");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        else
        {
            ModelState.Remove("Input.Username");

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {

                    _logger.LogInformation("User created a new account with password.");

                    await AddUserOrganisation(user);
                    await AddUserRoles(user);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    result = await _userManager.ConfirmEmailAsync(user, code);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return Page();
                    }

                    await _applicationDbContext.SetFullNameAsync(Input.Email, Input.Fullname);

                    return LocalRedirect($"~/Gds/Invitation/ConfirmUserAccountSetUp?username={Input.Email}");

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        
        
        // If we got this far, something failed, redisplay form
        return Page();
    }

    private ApplicationIdentityUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationIdentityUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationIdentityUser)}'. " +
                $"Ensure that '{nameof(ApplicationIdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<ApplicationIdentityUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationIdentityUser>)_userStore;
    }

    private async Task AddUserOrganisation(ApplicationIdentityUser user)
    {
        if (user == null)
            return;

        //if anything already exists then remove it.
        await _organisationRepository.DeleteUserByUserIdAsync(user.Id);

        if (Input.OrganisationId != null)
        {
            string[] parts = Input.OrganisationId.Split(',');
            if (parts != null && parts.Any())
            {
                foreach (string part in parts)
                {
                    await _organisationRepository.AddUserOrganisationAsync(new Models.Entities.UserOrganisation(Guid.NewGuid().ToString(), user.Id, part));
                }
            }
        }
    }

    private async Task AddUserRoles(ApplicationIdentityUser user)
    {
        if (string.IsNullOrEmpty(Input.Role))
        {
            return;
        }
        var roles = String.Join(", ", Input.Role.ToArray());
        var result = _userManager.AddClaimsAsync(user, new Claim[]
        {
            new Claim("UserId", user.Id),
            new Claim(JwtClaimTypes.Name, user.UserName),
            new Claim(JwtClaimTypes.GivenName, user.NormalizedUserName),
            new Claim(JwtClaimTypes.Role, roles)
        }).Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
        result = await _userManager.AddToRoleAsync(user, Input.Role);
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
    }

}

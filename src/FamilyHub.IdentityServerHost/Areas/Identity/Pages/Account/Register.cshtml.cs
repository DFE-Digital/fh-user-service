// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Data.AddOrganisation;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
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

        public List<OpenReferralOrganisationDto> UserOrganisations { get; set; } = default!;

        public RegisterModel(
            UserManager<ApplicationIdentityUser> userManager,
            IUserStore<ApplicationIdentityUser> userStore,
            SignInManager<ApplicationIdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IOrganisationRepository organisationRepository,
            IApiService apiService,
            IMemoryCache memoryCache)
        {
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
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public List<IdentityRole> AvailableRoles { get; set; } = default(List<IdentityRole>);

        [BindProperty]
        public List<string> RoleSelection { get; set; } = default!;

        [BindProperty]
        public string? Organisations { get; set; }

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
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null, string organisations = null)
        {
            ReturnUrl = returnUrl;
            Organisations = organisations;
            _memoryCache.Remove("OrganisationList");
            await Init();
        }

        private async Task Init()
        {
            if (User.IsInRole("DfEAdmin"))
                AvailableRoles = _roleManager.Roles.OrderBy(x => x.Name).ToList();
            else
                AvailableRoles = _roleManager.Roles.Where(x => x.Name != "DfEAdmin").OrderBy(x => x.Name).ToList();
            
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!User.IsInRole("DfEAdmin"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(userEmail);
            }

            if (!string.IsNullOrEmpty(Organisations))
            {
                string[] parts = Organisations.Split(',');
                if (parts != null && parts.Any())
                {
                    UserOrganisations = new List<OpenReferralOrganisationDto>();
                    foreach (string part in parts)
                    {
                        UserOrganisations.Add( await GetOrganisationById(part) );
                    }
                }
            }
        }

        private async Task<OpenReferralOrganisationDto> GetOrganisationById(string id)
        {
            if (_memoryCache.TryGetValue("OrganisationList", out List<OpenReferralOrganisationDto> cacheValue))
            {
                return cacheValue.FirstOrDefault(x => x.Id == id);
            }

            var organisationList = await _apiService.GetListOpenReferralOrganisations();

            TimeSpan ts = new TimeSpan(0, 5, 0);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(ts);

            _memoryCache.Set("OrganisationList", organisationList, cacheEntryOptions);

            return organisationList.FirstOrDefault(x => x.Id == id);


        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
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
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            if (!ModelState.IsValid)
            {
                await Init();
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

            if (Organisations != null)
            {
                string[] parts = Organisations.Split(',');
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
            if (RoleSelection == null || !RoleSelection.Any())
            {
                return;
            }
            var roles = String.Join(", ", RoleSelection.ToArray());
            var result = _userManager.AddClaimsAsync(user, new Claim[]
            {
        new Claim(JwtClaimTypes.Name, user.UserName),
        new Claim(JwtClaimTypes.GivenName, user.NormalizedUserName),
        new Claim(JwtClaimTypes.Role, roles),
        //new Claim(JwtClaimTypes.FamilyName, "Smith"),
        //new Claim(JwtClaimTypes.WebSite, "http://warmhandover.gov.uk"),
            }).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            foreach(var role in RoleSelection)
            {
                result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }
    }
}

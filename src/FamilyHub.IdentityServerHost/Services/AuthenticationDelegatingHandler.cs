using FamilyHub.IdentityServerHost.Api.Controllers;
using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.Services;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IOrganisationRepository _organisationRepository;

    
    public AuthenticationDelegatingHandler(ITokenService tokenService, 
                                           UserManager<ApplicationIdentityUser> userManager,
                                           RoleManager<IdentityRole> roleManager,
                                           IConfiguration configuration,
                                           IOrganisationRepository organisationRepository)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _organisationRepository = organisationRepository;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenService.GetToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            TokenModel? tokenModel = new TokenModel
            {
                AccessToken = token,
                RefreshToken = _tokenService.GetRefreshToken()
            };

            AuthenticateController authenticateController = new(
                                    _userManager,
                                    _roleManager,
                                    _configuration,
                                    _organisationRepository);

            ObjectResult? objectResult = await authenticateController.RefreshToken(tokenModel) as ObjectResult;
            if (objectResult != null && objectResult.Value != null) 
            {
                dynamic val = objectResult.Value;
                tokenModel = new TokenModel
                {
                    AccessToken = val.AccessToken,
                    RefreshToken = val.RefreshToken,
                };
            }
           
            if (tokenModel != null)
            {

                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(tokenModel.AccessToken);
                var claims = jwtSecurityToken.Claims.ToList();

                //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                var principal = new ClaimsPrincipal(identity);

                _tokenService.SetToken(tokenModel.AccessToken ?? string.Empty, jwtSecurityToken.ValidTo, tokenModel.RefreshToken ?? string.Empty);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{_tokenService.GetToken()}");
                response = await base.SendAsync(request, cancellationToken);
            }

        }

        return response;
    }
}

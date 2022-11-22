using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.Pages.Organisations;

public class ViewOrganisationListModel : PageModel
{
    private readonly IApiService _apiService;
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly IOrganisationRepository _organisationRepository;

    public record DisplayOrganisation : OpenReferralOrganisationDto
    {
        public string LocalAuthorityName { get; set; } = default!;
    }
    

    [BindProperty]
    public string Search { get; set; } = default!;

    [BindProperty]
    public int PageNumber { get; set; } = 1;
    [BindProperty]
    public int PageSize { get; set; } = 10;

    public int TotalPages { get; set; } = 1;

    public List<OpenReferralOrganisationDto> OpenReferralOrganisations { get; set; } = default!;
    public PaginatedList<DisplayOrganisation> PaginatedOpenReferralOrganisations { get; set; } = default!;

    public ViewOrganisationListModel(IApiService apiService, UserManager<ApplicationIdentityUser> userManager, IOrganisationRepository organisationRepository)
    {
        _apiService = apiService;
        _userManager = userManager;
        _organisationRepository = organisationRepository;
    }

    public async Task OnGet(string pageNumber)
    {
        if (!int.TryParse(pageNumber, out var page))
        {
            page = 1;
        }

        await GetOrganisations();

        var totalPages = OpenReferralOrganisations.Count() / PageSize;
        if (page < 1)
        {
            PageNumber = 1;
        }
        else if (page > totalPages)
        {
            PageNumber = totalPages;
        }
        else
        {
            PageNumber = page;
        }

        await GetPage();
    }

    public async Task OnPost()
    {
        await GetPage();
    }

    private async Task GetOrganisations()
    {
        if (OpenReferralOrganisations == null)
        {
            OpenReferralOrganisations = await _apiService.GetListOpenReferralOrganisations();

            //Just show organisations in the LA Area
            if (User.IsInRole("LAAdmin"))
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(userEmail);
                var organisationId = _organisationRepository.GetUserOrganisationIdByUserId(user.Id);
                var organisation = OpenReferralOrganisations.FirstOrDefault(x => x.Id == organisationId);
                if (organisation != null)
                {
                    OpenReferralOrganisations = OpenReferralOrganisations.Where(x => x.AdministractiveDistrictCode == organisation.AdministractiveDistrictCode).OrderBy(x => x.Name).ToList();
                }
            }
        }
    }

    private async Task GetPage()
    {
        await GetOrganisations();
        List<DisplayOrganisation> pagelist = default!;

        if (!string.IsNullOrEmpty(Search))
        {
            var allOrgs = OpenReferralOrganisations.Select(x => new DisplayOrganisation()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Logo = x.Logo,
                Uri = x.Uri,
                Url = x.Url,
                AdministractiveDistrictCode = x.AdministractiveDistrictCode,
                LocalAuthorityName = OpenReferralOrganisations.FirstOrDefault(y => y.AdministractiveDistrictCode == x.AdministractiveDistrictCode && y.OrganisationType.Name == "LA")?.Name ?? string.Empty,
            });

            pagelist = allOrgs.Where(x => x.Name!.Contains(Search) || x.LocalAuthorityName!.Contains(Search)).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        }
        else
        {
            pagelist = OpenReferralOrganisations.Select(x => new DisplayOrganisation()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Logo = x.Logo,
                Uri = x.Uri,
                Url = x.Url,
                AdministractiveDistrictCode = x.AdministractiveDistrictCode,
                LocalAuthorityName = OpenReferralOrganisations.FirstOrDefault(y => y.AdministractiveDistrictCode == x.AdministractiveDistrictCode && y.OrganisationType.Name == "LA")?.Name ?? string.Empty,
            }).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        }

        
        TotalPages = (int)Math.Ceiling((double)OpenReferralOrganisations.Count / (double)PageSize);
        PaginatedOpenReferralOrganisations = new PaginatedList<DisplayOrganisation>(pagelist, pagelist.Count, PageNumber, PageSize);
    }
}

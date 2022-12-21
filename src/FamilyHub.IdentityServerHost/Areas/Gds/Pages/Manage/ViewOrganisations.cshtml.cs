using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;

public class ViewOrganisationsModel : PageModel
{
    private readonly IApiService _apiService;

    private List<OpenReferralOrganisationDto> OpenReferralOrganisations { get; set; } = default!;
    public List<OrganisationTypeDto> OrganisationTypes { get; set; } = default!;
    [BindProperty]
    public List<string> SelectedOrganisationTypes { get; set; } = default!;

    public PaginatedList<OpenReferralOrganisationDto> PagedOrganisations { get; set; } = default!;

    [BindProperty]
    public int PageNumber { get; set; } = 1;
    [BindProperty]
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;

    public ViewOrganisationsModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGet(string pageNumber)
    {
        OpenReferralOrganisations = await _apiService.GetListOpenReferralOrganisations();
        if (!int.TryParse(pageNumber, out var page))
        {
            page = 1;
        }

        var totalPages = (int)Math.Ceiling((float)OpenReferralOrganisations.Count() / (float)PageSize);
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

    public async Task OnGetClearFilter()
    {
        if (SelectedOrganisationTypes != null)
        {
            SelectedOrganisationTypes.Clear();
        }
        OpenReferralOrganisations = await _apiService.GetListOpenReferralOrganisations();
        await GetPage();
    }

    public async Task OnPost()
    {
        OpenReferralOrganisations = await _apiService.GetListOpenReferralOrganisations();
        await GetPage();
    }

    private async Task GetPage()
    {
        List<OpenReferralOrganisationDto> pagelist;
        OrganisationTypes = await _apiService.GetListOrganisationTypes();
        

        if (SelectedOrganisationTypes != null && SelectedOrganisationTypes.Any())
        {
            IEnumerable<OpenReferralOrganisationDto> allPages = OpenReferralOrganisations.Where(x => SelectedOrganisationTypes.Contains(x.OrganisationType.Name ?? string.Empty));

            pagelist = allPages.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            TotalPages = (int)Math.Ceiling((double)allPages.Count() / (double)PageSize);
            PagedOrganisations = new PaginatedList<OpenReferralOrganisationDto>(pagelist, allPages.Count(), PageNumber, PageSize);
        }
        else
        {
            pagelist = OpenReferralOrganisations.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            TotalPages = (int)Math.Ceiling((double)OpenReferralOrganisations.Count / (double)PageSize);
            PagedOrganisations = new PaginatedList<OpenReferralOrganisationDto>(pagelist, pagelist.Count, PageNumber, PageSize);
        }
    }

}

using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Pages.Organisations;

public class ViewOrganisationListModel : PageModel
{
    private readonly IApiService _apiService;

    public List<OpenReferralOrganisationDto> OpenReferralOrganisations { get; set; } = default!;

    public ViewOrganisationListModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGet()
    {
        OpenReferralOrganisations = await _apiService.GetListOpenReferralOrganisations();
    }
}

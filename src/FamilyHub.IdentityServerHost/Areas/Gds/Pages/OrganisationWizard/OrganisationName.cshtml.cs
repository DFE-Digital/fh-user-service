using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using NuGet.Configuration;

namespace FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;

public class OrganisationNameModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    public NewOrganisation? NewOrganisation { get; set; } = default!;

    [Required]
    [BindProperty]
    public string Name { get; set; } = default!;

    public string HeadingLabelQuestion { get; set; } = default!;
    public string NameLabel { get; set; } = default!;

    public OrganisationNameModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void OnGet()
    {
        _redisCacheService.StoreCurrentPageName("/OrganisationWizard/OrganisationName");
        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
        if (NewOrganisation != null)
        {
            Name = NewOrganisation?.Name ?? string.Empty;
            
        }

        InitPage();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) 
        {
            InitPage();
            return Page();
        }

        NewOrganisation = _redisCacheService.RetrieveNewOrganisation();
        if (NewOrganisation != null)
        {
            NewOrganisation.Name = Name;
        }
        _redisCacheService.StoreNewOrganisation(NewOrganisation);

        return RedirectToPage("/OrganisationWizard/CheckOrganisationDetails", new
        {
            area = "Gds",
        });
    }

    private void InitPage()
    {
        HeadingLabelQuestion = "What is the local authority's name?";
        if (NewOrganisation != null && NewOrganisation.OrganisationTypeDto != null)
        {
            switch (NewOrganisation.OrganisationTypeDto.Name)
            {
                case "VCS":
                    HeadingLabelQuestion = "What is the Voluntary and community sector name?";
                    break;

                case "Company":
                    HeadingLabelQuestion = "What is the Company name?";
                    break;
            }
        }
    }
}

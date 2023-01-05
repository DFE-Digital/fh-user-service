using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Manage;

public class WhenUsingViewOrganisations
{
    private readonly ViewOrganisationsModel _viewOrganisationsModel;
    private readonly Mock<IApiService> _apiServiceMock;
    public WhenUsingViewOrganisations()
    {
        _apiServiceMock = new Mock<IApiService>();
        _viewOrganisationsModel = new ViewOrganisationsModel(_apiServiceMock.Object);

        OpenReferralOrganisationWithServicesDto org = MockOrganisation.GetTestCountyCouncilDto();
        OpenReferralOrganisationWithServicesDto vcs = MockOrganisation.GetTestVCSDto();

        List<OpenReferralOrganisationDto> listVcs = new()
        {
            new OpenReferralOrganisationDto(
                    id: org.Id,
                    organisationType: org.OrganisationType,
                    name: org.Name ?? default!,
                    description: org.Description,
                    logo: org.Logo,
                    uri: org.Uri,
                    url: org.Url),
            new OpenReferralOrganisationDto(
                id: vcs.Id,
                organisationType: vcs.OrganisationType,
                name: vcs.Name ?? default!,
                description: vcs.Description,
                logo: vcs.Logo,
                uri: vcs.Uri,
                url: vcs.Url)
        };

        listVcs[0].AdministractiveDistrictCode = "XTEST";
        listVcs[1].AdministractiveDistrictCode = "XTEST";

        _apiServiceMock.Setup(x => x.GetListOpenReferralOrganisations()).ReturnsAsync(listVcs);

        List<OrganisationTypeDto> organisationTypes = new()
        {
            new OrganisationTypeDto("1", "LA", "Local Authority"),
            new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
            new OrganisationTypeDto("3", "FamilyHub", "Family Hub"),
            new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre")
        };

        _apiServiceMock.Setup(x => x.GetListOrganisationTypes()).ReturnsAsync(organisationTypes);

    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    public async void ThenGettingViewOrganisationsPageWithoutSelection(string pageNumber)
    {
        //Act
        await _viewOrganisationsModel.OnGet(pageNumber);

        //Assert
        _viewOrganisationsModel.PagedOrganisations.Items.Count().Should().BeGreaterOrEqualTo(2);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("X")]
    public async void ThenGettingViewOrganisationsPageWithSelection(string pageNumber)
    {
        //Arrange
        _viewOrganisationsModel.SelectedOrganisationTypes = new List<string>
        {
            "LA"
        };

        //Act
        await _viewOrganisationsModel.OnGet(pageNumber);

        //Assert
        _viewOrganisationsModel.PagedOrganisations.Items.Count().Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async void ThenGettingClearFilterWithSelection()
    {
        //Arrange
        _viewOrganisationsModel.PageNumber = 1;
        _viewOrganisationsModel.SelectedOrganisationTypes = new List<string>
        {
            "LA"
        };

        //Act
        await _viewOrganisationsModel.OnGetClearFilter();

        //Assert
        _viewOrganisationsModel.PagedOrganisations.Items.Count().Should().BeGreaterOrEqualTo(2);
        _viewOrganisationsModel.SelectedOrganisationTypes.Any().Should().BeFalse();
    }

    [Fact]
    public async void ThenPostingViewOrganisationsPageWithoutSelection()
    {
        //Arrange
        _viewOrganisationsModel.PageNumber = 1;

        //Act
        await _viewOrganisationsModel.OnPost();

        //Assert
        _viewOrganisationsModel.PagedOrganisations.Items.Count().Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async void ThenPostingViewOrganisationsPageWithSelection()
    {
        //Arrange
        _viewOrganisationsModel.PageNumber = 1;
        _viewOrganisationsModel.SelectedOrganisationTypes = new List<string>
        {
            "LA"
        };

        //Act
        await _viewOrganisationsModel.OnPost();

        //Assert
        _viewOrganisationsModel.PagedOrganisations.Items.Count().Should().BeGreaterOrEqualTo(1);
    }
}

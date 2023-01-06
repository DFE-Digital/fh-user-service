using FamilyHub.IdentityServerHost.Areas.Gds.Pages.Manage;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FluentAssertions;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.Manage;

public class WhenUsingViewOrganisationDetail
{
    private readonly ViewOrganisationDetailModel _viewOrganisationDetailModel;
    private readonly Mock<IApiService> _apiServiceMock;
    public WhenUsingViewOrganisationDetail()
    {
        _apiServiceMock = new Mock<IApiService>();
        _viewOrganisationDetailModel = new ViewOrganisationDetailModel(_apiServiceMock.Object);


    }

    private void Setup(bool isLA)
    {
        OpenReferralOrganisationWithServicesDto org = MockOrganisation.GetTestCountyCouncilDto();
        if (isLA)
        {
            
            _apiServiceMock.Setup(x => x.GetOpenReferralOrganisationById(It.IsAny<string>())).ReturnsAsync(org);

            List<OpenReferralOrganisationDto> list = new()
            {
                new OpenReferralOrganisationDto(
                    id: org.Id,
                    organisationType: org.OrganisationType,
                    name: org.Name ?? default!,
                    description: org.Description,
                    logo: org.Logo,
                    uri: org.Uri,
                    url: org.Url                )
            };

            list[0].AdministractiveDistrictCode = "XTEST";

            _apiServiceMock.Setup(x => x.GetListOpenReferralOrganisations()).ReturnsAsync(list);
            return;
        }

        OpenReferralOrganisationWithServicesDto vcs = MockOrganisation.GetTestVCSDto();
        _apiServiceMock.Setup(x => x.GetOpenReferralOrganisationById(It.IsAny<string>())).ReturnsAsync(vcs);

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
    }

    [Theory]
    [InlineData(true, "56e62852-1b0b-40e5-ac97-54a67ea957dc")]
    [InlineData(false, "66fcee88-038a-463f-b913-ea3e96884a6d")]
    public async void ThenGettingViewOrganisationDetailPage(bool isLA, string organisationId)
    {
        //Arrange
        Setup(isLA);

        //Act
        await _viewOrganisationDetailModel.OnGet(organisationId);

        //Assert
        _viewOrganisationDetailModel.LocalAuthority.Should().Be("Unit Test County Council");

    }

    [Theory]
    [InlineData(true, "56e62852-1b0b-40e5-ac97-54a67ea957dc")]
    [InlineData(false, "66fcee88-038a-463f-b913-ea3e96884a6d")]
    public async void ThenPostingViewOrganisationDetailPageWithEmptyName(bool isLA, string organisationId)
    {
        //Arrange
        Setup(isLA);
        _viewOrganisationDetailModel.OrganisationId = organisationId;
        _viewOrganisationDetailModel.Name = default!;

        //Act
        await _viewOrganisationDetailModel.OnPost();

        //Assert
        _viewOrganisationDetailModel.ModelState.IsValid.Should().BeFalse();

    }

    [Theory]
    [InlineData(true, "56e62852-1b0b-40e5-ac97-54a67ea957dc")]
    [InlineData(false, "66fcee88-038a-463f-b913-ea3e96884a6d")]
    public async void ThenPostingViewOrganisationDetailPage(bool isLA, string organisationId)
    {
        //Arrange
        Setup(isLA);
        _viewOrganisationDetailModel.OrganisationId = organisationId;
        _viewOrganisationDetailModel.Name = "Test Organisation";
        _apiServiceMock.Setup(x => x.UpdateOrganisation(It.IsAny<OpenReferralOrganisationWithServicesDto>())).ReturnsAsync(organisationId);

        //Act
        await _viewOrganisationDetailModel.OnPost();

        //Assert
        _viewOrganisationDetailModel.ModelState.IsValid.Should().BeTrue();
        if (isLA)
            _viewOrganisationDetailModel.LocalAuthority.Should().Be("Test Organisation");
        else
            _viewOrganisationDetailModel.LocalAuthority.Should().Be("Unit Test County Council");

    }
}

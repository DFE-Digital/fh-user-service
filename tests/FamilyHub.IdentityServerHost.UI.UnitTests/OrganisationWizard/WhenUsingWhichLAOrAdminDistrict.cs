using FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;
using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.OrganisationWizard;

public class WhenUsingWhichLAOrAdminDistrict
{
    private readonly WhichLAOrAdminDistrictModel _whichLAOrAdminDistrictModel;
    private readonly Mock<IRedisCacheService> _redisCacheServiceMock;
    private readonly Mock<IApiService> _apiServiceMock;
    public WhenUsingWhichLAOrAdminDistrict()
    {
        _redisCacheServiceMock = new Mock<IRedisCacheService>();
        _apiServiceMock = new Mock<IApiService>();

        _redisCacheServiceMock.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        _redisCacheServiceMock.Setup(x => x.StoreNewOrganisation(It.IsAny<NewOrganisation>()));

        OpenReferralOrganisationWithServicesDto org = MockOrganisation.GetTestCountyCouncilDto();
        OpenReferralOrganisationWithServicesDto vcs = MockOrganisation.GetTestVCSDto();
        List<OpenReferralOrganisationDto> listOrg = new()
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

        listOrg[0].AdministractiveDistrictCode = "XTEST";
        listOrg[1].AdministractiveDistrictCode = "XTEST";

        _apiServiceMock.Setup(x => x.GetListOpenReferralOrganisations()).ReturnsAsync(listOrg);

        _whichLAOrAdminDistrictModel = new WhichLAOrAdminDistrictModel(_redisCacheServiceMock.Object, _apiServiceMock.Object);
    }

    [Theory]
    [InlineData("LA", "859e760d-3e4d-40dd-9a0e-efc97de73dcf")]
    [InlineData("VCFS", "859e760d-3e4d-40dd-9a0e-efc97de73dcf")]
    [InlineData("Company", "859e760d-3e4d-40dd-9a0e-efc97de73dcf")]
    public async Task ThenGettingWhichLAOrAdminDistrictPage(string name, string organisationCode)
    {
        //Arrange
        OrganisationTypeDto organisationTypeDto = name switch
        {
            "LA" => new OrganisationTypeDto("1", "LA", "Local Authority"),
            "VCFS" => new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
            _ => new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre"),
        };
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(new NewOrganisation { Name = "Test Organisation", OrganisationId = "859e760d-3e4d-40dd-9a0e-efc97de73dcf", ParentName = "ParentOrganisation", OrganisationTypeDto = organisationTypeDto });


        //Act
        await _whichLAOrAdminDistrictModel.OnGet();

        //Assert
        _whichLAOrAdminDistrictModel.OrganisationCode.Should().Be(organisationCode);


    }

    [Theory]
    [InlineData("LA", "859e760d-3e4d-40dd-9a0e-efc97de73dcf")]
    [InlineData("VCFS", "859e760d-3e4d-40dd-9a0e-efc97de73dcf")]
    [InlineData("Company", "859e760d-3e4d-40dd-9a0e-efc97de73dcf")]
    public async Task ThenPostingWhichLAOrAdminDistrictPageWithInvalidModel(string name, string organisationCode)
    {
        //Arrange
        OrganisationTypeDto organisationTypeDto = name switch
        {
            "LA" => new OrganisationTypeDto("1", "LA", "Local Authority"),
            "VCFS" => new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
            _ => new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre"),
        };
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(new NewOrganisation { Name = "Test Organisation", OrganisationId = "859e760d-3e4d-40dd-9a0e-efc97de73dcf", ParentName = "ParentOrganisation", OrganisationTypeDto = organisationTypeDto });
        _whichLAOrAdminDistrictModel.ModelState.AddModelError("OrganisationCode", "Required: OrganisationCode");

        //Act
        await _whichLAOrAdminDistrictModel.OnPost();

        //Assert
        _whichLAOrAdminDistrictModel.OrganisationCode.Should().Be(organisationCode);
        _whichLAOrAdminDistrictModel.ModelState.IsValid.Should().BeFalse();


    }

    [Theory]
    [InlineData("LA", "2bbd1e28-0064-448a-a120-27ab60715e48")]
    [InlineData("VCFS", "355b75d6-a567-4e44-b49e-62ef649ed391")]
    [InlineData("Company", "2c80b443-2c0f-47b9-a5bf-10f84274e342")]
    public async Task ThenPostingWhichLAOrAdminDistrictPageWithValidModel(string name, string organisationCode)
    {
        //Arrange
        OrganisationTypeDto organisationTypeDto = name switch
        {
            "LA" => new OrganisationTypeDto("1", "LA", "Local Authority"),
            "VCFS" => new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
            _ => new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre"),
        };

        if (name == "LA") 
        {
            _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(new NewOrganisation { Name = "Test Organisation", OrganisationId = "E06000001", ParentName = "ParentOrganisation", OrganisationTypeDto = organisationTypeDto });
        }
        else
        {
            _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(new NewOrganisation { Name = "Test Organisation", OrganisationId = Guid.NewGuid().ToString(), ParentName = "ParentOrganisation", OrganisationTypeDto = organisationTypeDto });
        }

        

        


        //Act
        var result = await _whichLAOrAdminDistrictModel.OnPost() as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/OrganisationWizard/OrganisationName");

    }
}

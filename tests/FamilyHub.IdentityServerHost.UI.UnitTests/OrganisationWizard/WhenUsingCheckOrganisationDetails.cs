using FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;
using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.OrganisationWizard;

public class WhenUsingCheckOrganisationDetails
{
    private readonly CheckOrganisationDetailsModel _checkOrganisationDetails;
    private readonly Mock<IRedisCacheService> _redisCacheServiceMock;
    private readonly Mock<IApiService> _apiServiceMock;
    public WhenUsingCheckOrganisationDetails()
    {
        _redisCacheServiceMock = new Mock<IRedisCacheService>();
        _apiServiceMock = new Mock<IApiService>();

        _redisCacheServiceMock.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        _redisCacheServiceMock.Setup(x => x.StoreNewOrganisation(It.IsAny<NewOrganisation>()));

        _checkOrganisationDetails = new CheckOrganisationDetailsModel(_redisCacheServiceMock.Object, _apiServiceMock.Object);
    }

    [Theory]
    [InlineData("LA")]
    [InlineData("VCFS")]
    [InlineData("Company")]
    public void ThenGettingCheckOrganisationDetailsPage(string name)
    {
        //Arrange
        OrganisationTypeDto organisationTypeDto = name switch
        {
            "LA" => new OrganisationTypeDto("1", "LA", "Local Authority"),
            "VCFS" => new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
            _ => new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre"),
        };
        NewOrganisation newOrganisation = new NewOrganisation { Name = "Test Organisation", OrganisationId = "859e760d-3e4d-40dd-9a0e-efc97de73dcf", ParentName = "ParentOrganisation", OrganisationTypeDto = organisationTypeDto };
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(newOrganisation);


        //Act
        _checkOrganisationDetails.OnGet();

        //Assert
        _checkOrganisationDetails.NewOrganisation.Should().NotBeNull();
        _checkOrganisationDetails.NewOrganisation.Should().BeEquivalentTo(newOrganisation);


    }

    [Theory]
    [InlineData("LA")]
    [InlineData("VCFS")]
    [InlineData("Company")]
    public async Task ThenPostingCheckOrganisationDetailsPage(string name)
    {
        //Arrange
        OrganisationTypeDto organisationTypeDto = name switch
        {
            "LA" => new OrganisationTypeDto("1", "LA", "Local Authority"),
            "VCFS" => new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
            _ => new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre"),
        };
        OpenReferralOrganisationWithServicesDto la = MockOrganisation.GetTestCountyCouncilDto();
        NewOrganisation newOrganisation;
        if (organisationTypeDto.Name == "LA")
            newOrganisation = new NewOrganisation { Name = "Test Organisation", OrganisationId = "859e760d-3e4d-40dd-9a0e-efc97de73dcf", ParentName = "ParentOrganisation", OrganisationTypeDto = organisationTypeDto };
        else
            newOrganisation = new NewOrganisation { Name = "Test Organisation", OrganisationId = la.Id, ParentName = la.Name ?? string.Empty, OrganisationTypeDto = organisationTypeDto };

        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(newOrganisation);

        _apiServiceMock.Setup(x => x.CreateOrganisation(It.IsAny<OpenReferralOrganisationWithServicesDto>())).ReturnsAsync("859e760d-3e4d-40dd-9a0e-efc97de73dcf");
        _apiServiceMock.Setup(x => x.GetOpenReferralOrganisationById(It.IsAny<string>())).ReturnsAsync(la);

        //Act
        var result = await _checkOrganisationDetails.OnPost() as RedirectToPageResult;

        //Assert
        _checkOrganisationDetails.NewOrganisation.Should().NotBeNull();
        _checkOrganisationDetails.NewOrganisation.Should().BeEquivalentTo(newOrganisation);
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/OrganisationWizard/Confirmation");
    }

    [Theory]
    [InlineData("VCFS")]
    [InlineData("Company")]
    public async Task ThenPostingCheckOrganisationDetailsPageWithNullParentLA(string name)
    {
        //Arrange
        OrganisationTypeDto organisationTypeDto = name switch
        {
            "LA" => new OrganisationTypeDto("1", "LA", "Local Authority"),
            "VCFS" => new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
            _ => new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre"),
        };
        OpenReferralOrganisationWithServicesDto la = default!;
        NewOrganisation newOrganisation = new NewOrganisation { Name = "Test Organisation", OrganisationId = default!, ParentName = default!, OrganisationTypeDto = organisationTypeDto };
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(newOrganisation);

        
        _apiServiceMock.Setup(x => x.GetOpenReferralOrganisationById(It.IsAny<string>())).ReturnsAsync(la);

        //Act
        var result = await _checkOrganisationDetails.OnPost() as PageResult;

        //Assert
        _checkOrganisationDetails.NewOrganisation.Should().NotBeNull();
        _checkOrganisationDetails.NewOrganisation.Should().BeEquivalentTo(newOrganisation);
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
    }
}

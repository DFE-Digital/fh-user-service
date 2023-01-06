using FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;
using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.OrganisationWizard;

public class WhenUsingTypeOfOrganisation
{
    private readonly TypeOfOrganisationModel _typeOfOrganisationModel;
    private readonly Mock<IRedisCacheService> _redisCacheServiceMock;
    private readonly Mock<IApiService> _apiServiceMock;
    public WhenUsingTypeOfOrganisation()
    {
        _redisCacheServiceMock = new Mock<IRedisCacheService>();
        _apiServiceMock = new Mock<IApiService>();

        _redisCacheServiceMock.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        _redisCacheServiceMock.Setup(x => x.StoreNewOrganisation(It.IsAny<NewOrganisation>()));

        _apiServiceMock.Setup(x => x.GetListOrganisationTypes()).ReturnsAsync(new List<OrganisationTypeDto>()
        {
            new OrganisationTypeDto("1", "LA", "Local Authority"),
            new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
            new OrganisationTypeDto("3", "FamilyHub", "Family Hub"),
            new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre")
        });

        _typeOfOrganisationModel = new TypeOfOrganisationModel(_redisCacheServiceMock.Object, _apiServiceMock.Object);
    }

    [Fact]
    public async Task ThenGettingTypeOfOrganisationPage()
    {

        //Arrange
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(new NewOrganisation { Name = "Test Organisation", OrganisationId = Guid.NewGuid().ToString(), ParentName = "ParentOrganisation" });

        //Act
        await _typeOfOrganisationModel.OnGet();

        //Assert
        _typeOfOrganisationModel.OrganisationTypes.Count().Should().BeGreaterOrEqualTo(3);
        _typeOfOrganisationModel.OrganisationTypes.Should().NotContain(x => x.Name == "FamilyHub");
    }

    [Fact]
    public async Task ThenPostingTypeOfOrganisationPageWithInvalidModel()
    {
        //Arrange
        NewOrganisation newOrganisation = default!;
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(newOrganisation);
        _typeOfOrganisationModel.ModelState.AddModelError("OrganisationTypeSelection", "Required: OrganisationTypeSelection");

        //Act
        await _typeOfOrganisationModel.OnPost();

        //Assert
        _typeOfOrganisationModel.ModelState.IsValid.Should().BeFalse();
        
    }

    [Fact]
    public async Task ThenPostingTypeOfOrganisationPage()
    {
        //Arrange
        NewOrganisation newOrganisation = default!;
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(newOrganisation);

        //Act
        var result = await _typeOfOrganisationModel.OnPost() as RedirectToPageResult;

        //Assert
        _typeOfOrganisationModel.ModelState.IsValid.Should().BeTrue();
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/OrganisationWizard/WhichLAOrAdminDistrict");

    }


}

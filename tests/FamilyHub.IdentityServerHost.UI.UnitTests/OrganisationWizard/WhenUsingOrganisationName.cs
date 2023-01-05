using FamilyHub.IdentityServerHost.Areas.Gds.Pages.OrganisationWizard;
using FamilyHub.IdentityServerHost.Models;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHub.IdentityServerHost.UI.UnitTests.OrganisationWizard;

public class WhenUsingOrganisationName
{
    private readonly OrganisationNameModel _organisationNameModel;
    private readonly Mock<IRedisCacheService> _redisCacheServiceMock;
    public WhenUsingOrganisationName()
    {
        _redisCacheServiceMock = new Mock<IRedisCacheService>();
        _redisCacheServiceMock.Setup(x => x.StoreCurrentPageName(It.IsAny<string>()));
        _redisCacheServiceMock.Setup(x => x.StoreNewOrganisation(It.IsAny<NewOrganisation>()));
        _organisationNameModel = new OrganisationNameModel(_redisCacheServiceMock.Object);
    }

    [Theory]
    [InlineData("LA", "What is the local authority's name?")]
    [InlineData("VCFS", "What is the Voluntary and community sector name?")]
    [InlineData("Company", "What is the Company name?")]

    public void ThenGettingOrganisationNamePage(string name, string headingName)
    {
        //Arrange
        OrganisationTypeDto organisationTypeDto;
        switch(name)
        {
            case "LA":
                organisationTypeDto = new OrganisationTypeDto("1", "LA", "Local Authority");
                break;
            case "VCFS":
                organisationTypeDto = new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector");
                break;
            default:
                organisationTypeDto = new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre");
                break;
        }
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(new NewOrganisation { Name = "Test Organisation", OrganisationId = Guid.NewGuid().ToString(), ParentName = "ParentOrganisation", OrganisationTypeDto = organisationTypeDto });

        //Act
        _organisationNameModel.OnGet();

        //Assert
        _organisationNameModel.HeadingLabelQuestion.Should().Be(headingName);
    }

    [Theory]
    [InlineData("LA", "What is the local authority's name?")]
    [InlineData("VCFS", "What is the Voluntary and community sector name?")]
    [InlineData("Company", "What is the Company name?")]

    public void ThenPostingOrganisationNamePageWithInvalidModel(string name, string headingName)
    {
        //Arrange
        OrganisationTypeDto organisationTypeDto;
        switch (name)
        {
            case "LA":
                organisationTypeDto = new OrganisationTypeDto("1", "LA", "Local Authority");
                break;
            case "VCFS":
                organisationTypeDto = new OrganisationTypeDto("2", "VCFS", "Voluntary, Charitable, Faith Sector");
                break;
            default:
                organisationTypeDto = new OrganisationTypeDto("4", "Company", "Public / Private Company eg: Child Care Centre");
                break;
        }

        _organisationNameModel.NewOrganisation = new NewOrganisation { Name = "Test Organisation", OrganisationId = Guid.NewGuid().ToString(), ParentName = "ParentOrganisation", OrganisationTypeDto = organisationTypeDto };
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(_organisationNameModel.NewOrganisation);
        _organisationNameModel.ModelState.AddModelError("Name", "Name is required");


        //Act
        var result = _organisationNameModel.OnPost() as RedirectToPageResult;

        //Assert
        _organisationNameModel.HeadingLabelQuestion.Should().Be(headingName);
        _organisationNameModel.ModelState.IsValid.Should().BeFalse();
        
    }

    [Fact]

    public void ThenPostingOrganisationNamePage()
    {
        //Arrange
        _organisationNameModel.Name = "Test Organisation";
        _organisationNameModel.NewOrganisation = new NewOrganisation { Name = "Test Organisation", OrganisationId = Guid.NewGuid().ToString(), ParentName = "ParentOrganisation", OrganisationTypeDto = new OrganisationTypeDto("1", "LA", "Local Authority") };
        _redisCacheServiceMock.Setup(x => x.RetrieveNewOrganisation()).Returns(_organisationNameModel.NewOrganisation);

        //Act
        var result = _organisationNameModel.OnPost() as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        result.PageName.Should().Be("/OrganisationWizard/CheckOrganisationDetails");
    }
}

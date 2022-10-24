using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using System.Text;
using System.Text.Json;

namespace FamilyHub.IdentityServerHost.Services;

public interface IApiService
{
    Task<List<OrganisationTypeDto>> GetListOrganisationTypes();
    Task<List<OpenReferralOrganisationDto>> GetListOpenReferralOrganisations();
    Task<OpenReferralOrganisationDto> GetOpenReferralOrganisationById(string id);
    Task<string> GetAdminCodeByOrganisationId(string organisationId);
    Task<string> CreateOrganisation(OpenReferralOrganisationWithServicesDto organisation);
    Task<string> UpdateOrganisation(OpenReferralOrganisationWithServicesDto organisation);
}

public class ApiService : IApiService
{
    protected readonly HttpClient _client;

    public ApiService(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<OrganisationTypeDto>> GetListOrganisationTypes()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/organizationtypes"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<OrganisationTypeDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrganisationTypeDto>();

    }

    public async Task<List<OpenReferralOrganisationDto>> GetListOpenReferralOrganisations()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/organizations"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<OpenReferralOrganisationDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OpenReferralOrganisationDto>();

    }

    public async Task<OpenReferralOrganisationDto> GetOpenReferralOrganisationById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/organizations/{id}"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<OpenReferralOrganisationDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new OpenReferralOrganisationDto();

    }

    public async Task<string> GetAdminCodeByOrganisationId(string organisationId)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/organizationAdminCode/{organisationId}"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }

    public async Task<string> CreateOrganisation(OpenReferralOrganisationWithServicesDto organisation)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + "api/organizations"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(organisation), Encoding.UTF8, "application/json"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }

    public async Task<string> UpdateOrganisation(OpenReferralOrganisationWithServicesDto organisation)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(_client.BaseAddress + $"api/organizations/{organisation.Id}"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(organisation), Encoding.UTF8, "application/json"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }
}
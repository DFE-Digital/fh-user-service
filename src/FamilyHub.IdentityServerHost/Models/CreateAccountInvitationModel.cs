using FamilyHub.IdentityServerHost.Helpers;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FamilyHub.IdentityServerHost.Models;

public class CreateAccountInvitationModel
{
    public CreateAccountInvitationModel()
    {

    }
    public CreateAccountInvitationModel(string organisationId, string role, DateTime dateExpired)
    {
        OrganisationId = organisationId;
        Role = role;
        DateExpired = dateExpired;
    }

    public string OrganisationId { get; init; } = default!;
    public string Role { get; init; } = default!;
    public DateTime DateExpired { get; init; }

    public static string GetTokenString(string key, string organisationId, string role, DateTime dateExpired)
    {
        CreateAccountInvitationModel createAccountInvitationModel = new CreateAccountInvitationModel(organisationId, role, dateExpired);
        var content = Newtonsoft.Json.JsonConvert.SerializeObject(createAccountInvitationModel);
        return Crypt.Encrypt(content, key); 
    }

    public static CreateAccountInvitationModel GetCreateAccountInvitationModel(string key, string tokenstring)
    {
        string json = Crypt.Decrypt(tokenstring, key);
        return  JsonSerializer.Deserialize<CreateAccountInvitationModel>(tokenstring) ?? new CreateAccountInvitationModel();
    }
}

using FamilyHub.IdentityServerHost.Helpers;
using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using System.Text.Json;

namespace FamilyHub.IdentityServerHost.Models;

public class CreateAccountInvitationModel
{
    public CreateAccountInvitationModel()
    {

    }
    public CreateAccountInvitationModel(string emailAddress, string organisationId, string role, DateTime dateExpired)
    {
        EmailAddress = emailAddress;
        OrganisationId = organisationId;
        Role = role;
        DateExpired = dateExpired;
    }

    public string EmailAddress { get; init; } = default!;
    public string OrganisationId { get; init; } = default!;
    public string Role { get; init; } = default!;
    public DateTime DateExpired { get; init; }

    public static string GetTokenString(string key, string emailAddress, string organisationId, string role, DateTime dateExpired)
    {
        CreateAccountInvitationModel createAccountInvitationModel = new CreateAccountInvitationModel(emailAddress, organisationId, role, dateExpired);
        var content = Newtonsoft.Json.JsonConvert.SerializeObject(createAccountInvitationModel);
        return Crypt.Encrypt(content, key); 
    }

    public static CreateAccountInvitationModel? GetCreateAccountInvitationModel(string key, string tokenstring)
    {
        string json = Crypt.Decrypt(tokenstring, key);
        json = json.Remove(json.Length - 3, 1);

        var microsoftDateFormatSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Local
        };

        CreateAccountInvitationModel? model = JsonConvert.DeserializeObject<CreateAccountInvitationModel>(json,
            microsoftDateFormatSettings);
            //new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ" });

        return model;        
    }
}

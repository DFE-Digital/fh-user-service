using FamilyHub.IdentityServerHost.Helpers;
using Newtonsoft.Json;

namespace FamilyHub.IdentityServerHost.Models;

public class CreateAccountInvitationModel
{
    public CreateAccountInvitationModel()
    {

    }
    public CreateAccountInvitationModel(string username, string emailAddress, string organisationId, string role, DateTime dateExpired)
    {
        Username = username;
        EmailAddress = emailAddress;
        OrganisationId = organisationId;
        Role = role;
        DateExpired = dateExpired;
    }

    public string Username { get; init; } = default!;
    public string EmailAddress { get; init; } = default!;
    public string OrganisationId { get; init; } = default!;
    public string Role { get; init; } = default!;
    public DateTime DateExpired { get; init; }

    public static string GetTokenString(string key, string username, string emailAddress, string organisationId, string role, DateTime dateExpired)
    {
        CreateAccountInvitationModel createAccountInvitationModel = new CreateAccountInvitationModel(username, emailAddress, organisationId, role, dateExpired);
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

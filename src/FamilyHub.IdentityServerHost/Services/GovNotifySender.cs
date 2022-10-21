using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Notify.Client;

namespace FamilyHub.IdentityServerHost.Services;

public class GovNotifySetting
{
    public string APIKey { get; set; } = default!;
    public string TemplateId { get; set; } = default!;
}

public class GovNotifySender : IEmailSender
{
    private readonly IOptions<GovNotifySetting> _govNotifySettings;

    public GovNotifySender(IOptions<GovNotifySetting> govNotifySettings)
    {
        _govNotifySettings = govNotifySettings;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Dictionary<String, dynamic> personalisation = new Dictionary<string, dynamic>
        {
            {"subject", subject},
            {"htmlMessage", htmlMessage}
        };

        var client = new NotificationClient(_govNotifySettings.Value.APIKey);
        await client.SendEmailAsync(
                emailAddress: email,
                templateId: _govNotifySettings.Value.TemplateId,
                personalisation: personalisation
        );
    }
}

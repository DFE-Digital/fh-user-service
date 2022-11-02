using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Net.Mail;

namespace FamilyHub.IdentityServerHost.Services;

public class EmailSetting
{
    public bool IsEmailEnabled { get; set; }
    public string FromEmail { get; set; } = "donot-reply@emailserver.com";
    public int Port { get; set; } = 587;
    public string SMTPServer { get; set; } = "smtp.gmail.com";
    public string EmailServerUserName { get; set; } = default!;
    public string EmailServerPassword { get; set; } = default!;

}

public class EmailSender : IEmailSender
{
    private readonly IOptions<EmailSetting> _emailSettings;
    public EmailSender(IOptions<EmailSetting> emailSettings)
    {
        
        _emailSettings = emailSettings;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        //throw new NotImplementedException();

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.Value.FromEmail),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        using (var smtpClient = new SmtpClient(_emailSettings.Value.SMTPServer)
        {
            Port = _emailSettings.Value.Port,
            Credentials = new NetworkCredential(_emailSettings.Value.EmailServerUserName, _emailSettings.Value.EmailServerPassword),
            EnableSsl = true,
        })
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

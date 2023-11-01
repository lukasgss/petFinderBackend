using System.Text.Encodings.Web;
using Application.Common.Interfaces.Messaging;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Messaging;

public class MessagingService : IMessagingService
{
    private readonly MessagingSettings _messagingSettings;

    public MessagingService(IOptions<MessagingSettings> messagingSettings)
    {
        _messagingSettings = messagingSettings.Value;
    }

    public async Task SendAccountConfirmationMessageAsync(string userEmail, string confirmationLink)
    {
        MimeMessage email = new();
        email.From.Add(MailboxAddress.Parse(_messagingSettings.From));
        email.To.Add(MailboxAddress.Parse(userEmail));
        email.Subject = "Confirmação de Cadastro";
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = $@"Please confirm your account by visiting this url:
                     {HtmlEncoder.Default.Encode(confirmationLink)}"
        };

        using SmtpClient smtp = new();
        await smtp.ConnectAsync(_messagingSettings.Host, _messagingSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_messagingSettings.Username, _messagingSettings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
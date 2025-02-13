using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using UMS.Contracts;

namespace UMS.Features;

public class SmtpEmailService : IEmailService
{
    private readonly string _mailtrapHost;
    private readonly int _mailtrapPort;
    private readonly string _mailtrapUsername;
    private readonly string _mailtrapPassword;

    public SmtpEmailService(string mailtrapHost, int mailtrapPort, string mailtrapUsername, string mailtrapPassword)
    {
        _mailtrapHost = mailtrapHost;
        _mailtrapPort = mailtrapPort;
        _mailtrapUsername = mailtrapUsername;
        _mailtrapPassword = mailtrapPassword;
    }

    public async Task SendEmail(
        string subject,
        string plainBody,
        string htmlBody,
        (string EmailAddress, string Name) from,
        IEnumerable<(string EmailAddress, string Name)> to,
        IEnumerable<(string EmailAddress, string Name)>? cc = null,
        IEnumerable<(string EmailAddress, string Name)>? bcc = null,
        CancellationToken cancellationToken = default)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(from.Name, from.EmailAddress));

        foreach ((string EmailAddress, string Name) recipient in to)
        {
            message.To.Add(new MailboxAddress(recipient.Name, recipient.EmailAddress));
        }

        if (cc != null)
        {
            foreach ((string EmailAddress, string Name) ccRecipient in cc)
            {
                message.Cc.Add(new MailboxAddress(ccRecipient.Name, ccRecipient.EmailAddress));
            }
        }

        if (bcc != null)
        {
            foreach ((string EmailAddress, string Name) bccRecipient in bcc)
            {
                message.Bcc.Add(new MailboxAddress(bccRecipient.Name, bccRecipient.EmailAddress));
            }
        }

        message.Subject = subject;

        BodyBuilder bodyBuilder = new BodyBuilder
        {
            TextBody = plainBody,
            HtmlBody = htmlBody
        };

        message.Body = bodyBuilder.ToMessageBody();

        using (SmtpClient client = new SmtpClient())
        {
            await client.ConnectAsync(_mailtrapHost, _mailtrapPort, SecureSocketOptions.StartTls, cancellationToken);

            await client.AuthenticateAsync(_mailtrapUsername, _mailtrapPassword, cancellationToken);

            await client.SendAsync(message, cancellationToken);

            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
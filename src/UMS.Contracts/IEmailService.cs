namespace UMS.Contracts;

public interface IEmailService
{
    Task SendEmail(
        string subject,
        string plainBody,
        string htmlBody,
        (string EmailAddress, string Name) from,
        IEnumerable<(string EmailAddress, string Name)> to,
        IEnumerable<(string EmailAddress, string Name)>? cc = null,
        IEnumerable<(string EmailAddress, string Name)>? bcc = null,
        CancellationToken cancellationToken = default);
}
using UMS.Contracts;

namespace UMS.Features;

public class OtpService : IOtpService
{
    private static readonly Random GenerateRandomToken = new();
    private readonly ICache _cache;
    private readonly IEmailService _emailService;

    public OtpService(ICache cache, IEmailService emailService)
    {
        _cache = cache;
        _emailService = emailService;
    }

    public async Task GenerateAndSendOtp(string emailAddress, OtpPurpose purpose)
    {
        string otp = CreateRandomToken();

        TimeSpan otpValidityDuration = OtpConstants.Durations.TryGetValue(purpose, out TimeSpan purposeDuration)
            ? purposeDuration
            : TimeSpan.Zero;

        await _cache.Write(emailAddress, otp, ttl: otpValidityDuration);

        await SendOtpByEmail(emailAddress, otp, otpValidityDuration);
    }

    public async Task<bool> Exists(string emailAddress, string otp)
    {
        (bool exists, TimeSpan? expiresIn) exists = await _cache.Exists(emailAddress);

        if (!exists.exists)
        {
            return false;
        }

        (string Value, TimeSpan ExpiresIn)? cachedOtp = await _cache.Read(emailAddress);

        return cachedOtp?.Value == otp;
    }

    private string CreateRandomToken()
    {
        int CreateRandomNumber(int min, int max)
        {
            lock (GenerateRandomToken)
            {
                return GenerateRandomToken.Next(min, max);
            }
        }

        return CreateRandomNumber(100000, 999999).ToString("D6");
    }

    private async Task SendOtpByEmail(string emailAddress, string otp, TimeSpan otpValidityDuration)
    {
        string subject = "Your OTP Code";
        string plainBody = $"Your OTP code is: {otp} Valid for {otpValidityDuration.TotalMinutes} minutes.";
        string htmlBody = $"<p>Your OTP code is: {otp} Valid for {otpValidityDuration.TotalMinutes} minutes.</p>";

        (string EmailAddress, string Name) from = ("UMS@example.com", "UMS");
        (string EmailAddress, string Name) to = (emailAddress, "Recipient Name");

        await _emailService.SendEmail(subject, plainBody, htmlBody, from, new[] { to });
    }
}
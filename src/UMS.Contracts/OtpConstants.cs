namespace UMS.Contracts;

public static class OtpConstants
{
    public static readonly Dictionary<OtpPurpose, TimeSpan> Durations = new()
    {
        [OtpPurpose.Registration] = TimeSpan.FromMinutes(20),
        [OtpPurpose.ForgotPassword] = TimeSpan.FromMinutes(5)
    };
}
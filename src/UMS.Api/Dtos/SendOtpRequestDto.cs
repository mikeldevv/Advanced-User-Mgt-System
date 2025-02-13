using UMS.Contracts;

namespace UMS.Api.Dtos;

public class SendOtpRequestDto
{
    public string EmailAddress { get; set; } = string.Empty;

    public OtpPurpose Purpose { get; set; }
}
namespace UMS.Api.Dtos;

public class ResetPasswordRequestDto
{
    public string Otp { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
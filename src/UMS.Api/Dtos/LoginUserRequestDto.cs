using System.ComponentModel.DataAnnotations;

namespace UMS.Api.Dtos;

public class LoginUserRequestDto
{
    [Required] public string EmailAddress { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}
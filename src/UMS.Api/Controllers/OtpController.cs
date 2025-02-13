using Microsoft.AspNetCore.Mvc;
using UMS.Api.Dtos;
using UMS.Contracts;

namespace UMS.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class OtpController : AbstractController
{
    private readonly IOtpService _otpService;

    public OtpController(IOtpService otpService)
    {
        _otpService = otpService;
    }

    [HttpPost]
    public async Task<IActionResult> SendOtpViaEmail(SendOtpRequestDto sendOtpRequestDto)
    {
        await _otpService.GenerateAndSendOtp(sendOtpRequestDto.EmailAddress, sendOtpRequestDto.Purpose);

        return Ok();
    }
}
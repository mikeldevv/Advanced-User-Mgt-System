namespace UMS.Contracts;

public interface IOtpService
{
    /// <summary>
    /// Generates an OTP (One-Time Password) and sends it to the provided email address.
    /// The OTP duration is determined based on the specified purpose.
    /// </summary>
    /// <param name="emailAddress">The email address to which the OTP will be sent.</param>
    /// <param name="purpose">The purpose for which the OTP is generated.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task GenerateAndSendOtp(string emailAddress, OtpPurpose purpose);
    
    /// <summary>
    /// Checks if the provided OTP exists in the cache for the specified email address.
    /// </summary>
    /// <param name="emailAddress">The email address to check for OTP existence.</param>
    /// <param name="otp">The OTP to be checked.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result is a boolean indicating whether the OTP exists and matches the provided OTP.</returns>
    Task<bool> Exists(string emailAddress, string otp);
    
}
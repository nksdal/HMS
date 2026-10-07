using HMS.Application.DTOs.Auth;
using HMS.Application.DTOs.Common;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Public registration. Always creates a Guest account with an unverified email.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registration successful. Please verify your email."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    /// <summary>Called by the confirm-email.html page when the user clicks "Verify My Account".</summary>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
    {
        await _authService.VerifyEmailAsync(dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Email verified successfully."));
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(ResendVerificationDto dto)
    {
        await _authService.ResendVerificationAsync(dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Verification email sent."));
    }
}

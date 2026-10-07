using HMS.Application.DTOs.Auth;

namespace HMS.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task VerifyEmailAsync(VerifyEmailDto dto);
    Task ResendVerificationAsync(ResendVerificationDto dto);
}

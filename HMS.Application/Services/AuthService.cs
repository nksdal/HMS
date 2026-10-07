using System.Security.Cryptography;
using System.Text;
using HMS.Application.DTOs.Auth;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace HMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IGuestRepository guestRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailSender emailSender,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _guestRepository = guestRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
            throw new ConflictException($"Email '{dto.Email}' is already registered.");

        if (await _userRepository.ExistsByPersonalNumberAsync(dto.PersonalNumber))
            throw new ConflictException($"Personal number '{dto.PersonalNumber}' is already registered.");

        if (await _userRepository.ExistsByPhoneNumberAsync(dto.PhoneNumber))
            throw new ConflictException($"Phone number '{dto.PhoneNumber}' is already registered.");

        var (hash, salt) = _passwordHasher.HashPassword(dto.Password);

        // Every self-registered user also gets a linked Guest business record.
        var guest = new Guest
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PersonalNumber = dto.PersonalNumber,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };
        await _guestRepository.AddAsync(guest);
        await _guestRepository.SaveChangesAsync();

        var (rawToken, tokenHash) = GenerateVerificationToken();

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PersonalNumber = dto.PersonalNumber,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Guest, // self-registration is ALWAYS Guest
            EmailConfirmed = false,
            EmailVerificationTokenHash = tokenHash,
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24),
            IsActive = true,
            GuestId = guest.Id
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        // A failed email must not make a successful registration look like a failure.
        try
        {
            await _emailSender.SendEmailConfirmationAsync(user.Email, rawToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to {Email}", user.Email);
            _logger.LogWarning("DEV FALLBACK - verification token for {Email}: {Token}", user.Email, rawToken);
        }

        var (jwt, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = jwt,
            ExpiresAt = expiresAt,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new BusinessRuleException("Invalid email or password.");

        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
            throw new BusinessRuleException("Invalid email or password.");

        if (!user.IsActive)
            throw new BusinessRuleException("This account has been deactivated.");

        if (!user.EmailConfirmed)
            throw new EmailNotVerifiedException("Email address has not been verified.");

        var (jwt, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = jwt,
            ExpiresAt = expiresAt,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        };
    }

    public async Task VerifyEmailAsync(VerifyEmailDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new NotFoundException(nameof(User), dto.Email);

        if (user.EmailConfirmed)
            throw new BusinessRuleException("Email is already verified.");

        if (user.EmailVerificationTokenHash is null || user.EmailVerificationTokenExpiresAt is null)
            throw new BusinessRuleException("No verification is pending for this account.");

        if (user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
            throw new BusinessRuleException("Verification link has expired. Please request a new one.");

        var incomingHash = HashToken(dto.Token);

        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(incomingHash),
                Convert.FromBase64String(user.EmailVerificationTokenHash)))
        {
            throw new BusinessRuleException("Invalid verification token.");
        }

        // Single use: the token is cleared once consumed.
        user.EmailConfirmed = true;
        user.EmailVerificationTokenHash = null;
        user.EmailVerificationTokenExpiresAt = null;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task ResendVerificationAsync(ResendVerificationDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new NotFoundException(nameof(User), dto.Email);

        if (user.EmailConfirmed)
            throw new BusinessRuleException("Email is already verified.");

        var (rawToken, tokenHash) = GenerateVerificationToken();

        user.EmailVerificationTokenHash = tokenHash;
        user.EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        try
        {
            await _emailSender.SendEmailConfirmationAsync(user.Email, rawToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resend verification email to {Email}", user.Email);
            _logger.LogWarning("DEV FALLBACK - verification token for {Email}: {Token}", user.Email, rawToken);
            throw new BusinessRuleException("Could not send verification email. Please try again later.");
        }
    }

    private static (string RawToken, string TokenHash) GenerateVerificationToken()
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        return (rawToken, HashToken(rawToken));
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}

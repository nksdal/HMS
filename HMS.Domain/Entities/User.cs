using HMS.Domain.Common;
using HMS.Domain.Enums;

namespace HMS.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PersonalNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Guest;

    public bool EmailConfirmed { get; set; } = false;
    public string? EmailVerificationTokenHash { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;

    public int? GuestId { get; set; }
    public Guest? Guest { get; set; }

    public int? ManagerId { get; set; }
    public Manager? Manager { get; set; }
}

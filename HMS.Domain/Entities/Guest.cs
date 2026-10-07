using HMS.Domain.Common;

namespace HMS.Domain.Entities;

public class Guest : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PersonalNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? IdentityDocumentNumber { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

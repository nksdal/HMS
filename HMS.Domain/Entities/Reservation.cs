using HMS.Domain.Common;
using HMS.Domain.Enums;

namespace HMS.Domain.Entities;

public class Reservation : BaseEntity
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public decimal TotalPrice { get; set; }

    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
}

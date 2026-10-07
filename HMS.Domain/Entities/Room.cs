using HMS.Domain.Common;
using HMS.Domain.Enums;

namespace HMS.Domain.Entities;

public class Room : BaseEntity
{
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }

    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

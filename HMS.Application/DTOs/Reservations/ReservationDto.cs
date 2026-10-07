using HMS.Domain.Enums;

namespace HMS.Application.DTOs.Reservations;

public class ReservationDto
{
    public int Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public ReservationStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public int GuestId { get; set; }
    public int RoomId { get; set; }
}

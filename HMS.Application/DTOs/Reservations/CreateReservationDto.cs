namespace HMS.Application.DTOs.Reservations;

public class CreateReservationDto
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int GuestId { get; set; }
    public int RoomId { get; set; }
}

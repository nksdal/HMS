using HMS.Domain.Enums;

namespace HMS.Application.DTOs.Rooms;

public class CreateRoomDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public int HotelId { get; set; }
}

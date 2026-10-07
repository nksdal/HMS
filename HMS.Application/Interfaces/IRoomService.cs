using HMS.Application.DTOs.Rooms;

namespace HMS.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllAsync();
    Task<IEnumerable<RoomDto>> GetByHotelIdAsync(int hotelId);
    Task<RoomDto> GetByIdAsync(int id);
    Task<RoomDto> CreateAsync(CreateRoomDto dto);
    Task UpdateAsync(int id, CreateRoomDto dto);
    Task DeleteAsync(int id);
}

using HMS.Application.DTOs.Hotels;

namespace HMS.Application.Interfaces;

public interface IHotelService
{
    Task<IEnumerable<HotelDto>> GetAllAsync();
    Task<HotelDto> GetByIdAsync(int id);
    Task<HotelDto> CreateAsync(CreateHotelDto dto);
    Task UpdateAsync(int id, CreateHotelDto dto);
    Task DeleteAsync(int id);
}

using HMS.Application.DTOs.Managers;

namespace HMS.Application.Interfaces;

public interface IManagerService
{
    Task<IEnumerable<ManagerDto>> GetByHotelIdAsync(int hotelId);
    Task<ManagerDto> GetByIdAsync(int id);
    Task<ManagerDto> CreateAsync(int hotelId, CreateManagerDto dto);
    Task DeleteAsync(int id);
}

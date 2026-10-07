using HMS.Application.DTOs.Guests;

namespace HMS.Application.Interfaces;

public interface IGuestService
{
    Task<IEnumerable<GuestDto>> GetAllAsync();
    Task<GuestDto> GetByIdAsync(int id);
    Task<GuestDto> CreateAsync(CreateGuestDto dto);
    Task UpdateAsync(int id, CreateGuestDto dto);
    Task DeleteAsync(int id);
}

using HMS.Application.DTOs.Reservations;

namespace HMS.Application.Interfaces;

public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllAsync();
    Task<ReservationDto> GetByIdAsync(int id);
    Task<ReservationDto> CreateAsync(CreateReservationDto dto);
    Task CancelAsync(int id);
}

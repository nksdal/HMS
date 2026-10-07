using HMS.Domain.Entities;

namespace HMS.Application.Interfaces;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByGuestIdAsync(int guestId);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
}

using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(HmsDbContext context) : base(context) { }

    public async Task<IEnumerable<Reservation>> GetByGuestIdAsync(int guestId) =>
        await _dbSet.Where(r => r.GuestId == guestId).ToListAsync();

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var overlapping = await _dbSet.AnyAsync(r =>
            r.RoomId == roomId &&
            r.Status != ReservationStatus.Cancelled &&
            checkIn < r.CheckOutDate &&
            checkOut > r.CheckInDate);

        return !overlapping;
    }
}

using HMS.Domain.Entities;

namespace HMS.Application.Interfaces;

public interface IHotelRepository : IGenericRepository<Hotel>
{
    Task<Hotel?> GetWithRoomsAsync(int hotelId);
}

using HMS.Domain.Entities;

namespace HMS.Application.Interfaces;

public interface IManagerRepository : IGenericRepository<Manager>
{
    Task<IEnumerable<Manager>> GetByHotelIdAsync(int hotelId);
}

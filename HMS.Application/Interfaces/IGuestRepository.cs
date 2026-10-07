using HMS.Domain.Entities;

namespace HMS.Application.Interfaces;

public interface IGuestRepository : IGenericRepository<Guest>
{
    Task<Guest?> GetByEmailAsync(string email);
}

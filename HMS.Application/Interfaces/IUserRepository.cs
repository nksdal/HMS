using HMS.Domain.Entities;

namespace HMS.Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByPersonalNumberAsync(string personalNumber);
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);
}

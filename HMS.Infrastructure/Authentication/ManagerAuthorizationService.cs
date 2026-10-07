using HMS.Application.Interfaces;
using HMS.Domain.Enums;

namespace HMS.Infrastructure.Authentication;

/// <summary>
/// Resolves hotel ownership from the database using the authenticated user's identity.
/// Client-supplied hotel ids are only ever compared against this, never trusted as proof.
/// </summary>
public class ManagerAuthorizationService : IManagerAuthorizationService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IManagerRepository _managerRepository;

    public ManagerAuthorizationService(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IManagerRepository managerRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _managerRepository = managerRepository;
    }

    public async Task<bool> CanManageHotelAsync(int hotelId)
    {
        if (_currentUserService.Role == UserRole.Admin.ToString())
            return true;

        var ownedHotelId = await GetOwnedHotelIdAsync();
        return ownedHotelId is not null && ownedHotelId == hotelId;
    }

    public async Task<int?> GetOwnedHotelIdAsync()
    {
        if (_currentUserService.UserId is not int userId)
            return null;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.ManagerId is not int managerId)
            return null;

        var manager = await _managerRepository.GetByIdAsync(managerId);
        return manager?.HotelId;
    }
}

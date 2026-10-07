namespace HMS.Application.Interfaces;

public interface IManagerAuthorizationService
{
    /// <summary>
    /// True if the current user is an Admin, or a Manager whose own hotel matches hotelId.
    /// Ownership is resolved from the database, never from client-supplied values.
    /// </summary>
    Task<bool> CanManageHotelAsync(int hotelId);

    /// <summary>
    /// The HotelId owned by the current Manager, or null if the user is not a Manager.
    /// </summary>
    Task<int?> GetOwnedHotelIdAsync();
}

using HMS.Application.DTOs.Common;
using HMS.Application.DTOs.Reservations;
using HMS.Application.Interfaces;
using HMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;

    public ReservationsController(
        IReservationService reservationService,
        ICurrentUserService currentUser,
        IUserRepository userRepository)
    {
        _reservationService = reservationService;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<ReservationDto>>.Ok(await _reservationService.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);

        if (IsGuest && await GetCallerGuestIdAsync() != reservation.GuestId)
            return Forbid();

        return Ok(ApiResponse<ReservationDto>.Ok(reservation));
    }

    [HttpPost]
    [Authorize(Roles = "Guest,Admin")]
    public async Task<IActionResult> Create(CreateReservationDto dto)
    {
        // A Guest may only book for themselves - the GuestId in the body is never trusted.
        if (IsGuest)
        {
            var myGuestId = await GetCallerGuestIdAsync();
            if (myGuestId is null || myGuestId != dto.GuestId)
                return Forbid();
        }

        var created = await _reservationService.CreateAsync(dto);
        return Ok(ApiResponse<ReservationDto>.Ok(created, "Reservation created."));
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);

        if (IsGuest && await GetCallerGuestIdAsync() != reservation.GuestId)
            return Forbid();

        await _reservationService.CancelAsync(id);
        return Ok(ApiResponse<object>.Ok(new { }, "Reservation cancelled."));
    }

    private bool IsGuest => _currentUser.Role == UserRole.Guest.ToString();

    private async Task<int?> GetCallerGuestIdAsync()
    {
        if (_currentUser.UserId is not int userId) return null;
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.GuestId;
    }
}

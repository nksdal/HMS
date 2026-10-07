using HMS.Application.DTOs.Common;
using HMS.Application.DTOs.Rooms;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IManagerAuthorizationService _managerAuth;

    public RoomsController(IRoomService roomService, IManagerAuthorizationService managerAuth)
    {
        _roomService = roomService;
        _managerAuth = managerAuth;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<RoomDto>>.Ok(await _roomService.GetAllAsync()));

    [HttpGet("hotel/{hotelId:int}")]
    public async Task<IActionResult> GetByHotel(int hotelId)
        => Ok(ApiResponse<IEnumerable<RoomDto>>.Ok(await _roomService.GetByHotelIdAsync(hotelId)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(ApiResponse<RoomDto>.Ok(await _roomService.GetByIdAsync(id)));

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(CreateRoomDto dto)
    {
        if (!await _managerAuth.CanManageHotelAsync(dto.HotelId))
            return Forbid();

        var created = await _roomService.CreateAsync(dto);
        return Ok(ApiResponse<RoomDto>.Ok(created, "Room created."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int id, CreateRoomDto dto)
    {
        // Check ownership of BOTH the room's current hotel and the target hotel.
        var existing = await _roomService.GetByIdAsync(id);

        if (!await _managerAuth.CanManageHotelAsync(existing.HotelId))
            return Forbid();

        if (!await _managerAuth.CanManageHotelAsync(dto.HotelId))
            return Forbid();

        await _roomService.UpdateAsync(id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Room updated."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _roomService.GetByIdAsync(id);

        if (!await _managerAuth.CanManageHotelAsync(room.HotelId))
            return Forbid();

        await _roomService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(new { }, "Room deleted."));
    }
}

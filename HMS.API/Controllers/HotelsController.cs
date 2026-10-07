using HMS.Application.DTOs.Common;
using HMS.Application.DTOs.Hotels;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IManagerAuthorizationService _managerAuth;

    public HotelsController(IHotelService hotelService, IManagerAuthorizationService managerAuth)
    {
        _hotelService = hotelService;
        _managerAuth = managerAuth;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<HotelDto>>.Ok(await _hotelService.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(ApiResponse<HotelDto>.Ok(await _hotelService.GetByIdAsync(id)));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateHotelDto dto)
    {
        var created = await _hotelService.CreateAsync(dto);
        return Ok(ApiResponse<HotelDto>.Ok(created, "Hotel created."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int id, CreateHotelDto dto)
    {
        if (!await _managerAuth.CanManageHotelAsync(id))
            return Forbid();

        await _hotelService.UpdateAsync(id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Hotel updated."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _hotelService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(new { }, "Hotel deleted."));
    }
}

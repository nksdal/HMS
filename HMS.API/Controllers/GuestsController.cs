using HMS.Application.DTOs.Common;
using HMS.Application.DTOs.Guests;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;

    public GuestsController(IGuestService guestService)
    {
        _guestService = guestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<GuestDto>>.Ok(await _guestService.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(ApiResponse<GuestDto>.Ok(await _guestService.GetByIdAsync(id)));

    [HttpPost]
    public async Task<IActionResult> Create(CreateGuestDto dto)
    {
        var created = await _guestService.CreateAsync(dto);
        return Ok(ApiResponse<GuestDto>.Ok(created, "Guest created."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateGuestDto dto)
    {
        await _guestService.UpdateAsync(id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Guest updated."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _guestService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(new { }, "Guest deleted."));
    }
}

using HMS.Application.DTOs.Common;
using HMS.Application.DTOs.Managers;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class ManagersController : ControllerBase
{
    private readonly IManagerService _managerService;
    private readonly IManagerAuthorizationService _managerAuth;

    public ManagersController(IManagerService managerService, IManagerAuthorizationService managerAuth)
    {
        _managerService = managerService;
        _managerAuth = managerAuth;
    }

    [HttpGet("hotels/{hotelId:int}/managers")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetByHotel(int hotelId)
    {
        if (!await _managerAuth.CanManageHotelAsync(hotelId))
            return Forbid();

        return Ok(ApiResponse<IEnumerable<ManagerDto>>.Ok(await _managerService.GetByHotelIdAsync(hotelId)));
    }

    /// <summary>Admin-only. Creates the Manager record AND its login account in one step.</summary>
    [HttpPost("hotels/{hotelId:int}/managers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(int hotelId, CreateManagerDto dto)
    {
        var created = await _managerService.CreateAsync(hotelId, dto);
        return Ok(ApiResponse<ManagerDto>.Ok(created, "Manager created successfully."));
    }

    [HttpGet("managers/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetById(int id)
        => Ok(ApiResponse<ManagerDto>.Ok(await _managerService.GetByIdAsync(id)));

    [HttpDelete("managers/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _managerService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(new { }, "Manager deleted."));
    }
}

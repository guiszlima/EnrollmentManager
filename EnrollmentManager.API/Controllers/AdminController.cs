using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Dtos.User;
using EnrollmentManager.API.Services.Admin;
using EnrollmentManager.API.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IPasswordResetService _passwordResetService;

    public AdminController(IAdminService adminService, IPasswordResetService passwordResetService)
    {
        _passwordResetService = passwordResetService;
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponseDto<List<AdminUserDto>>>> GetUsersAsync(
        [FromQuery] bool active)
    {
        var response = await _adminService.GetUsersAsync(active);

        return Ok(response);
    }

    [HttpPatch("users/{id}/role")]
    public async Task<ActionResult<ApiResponseDto<AdminUserDto>>> ChangeUserRoleAsync(
        int id,
        ChangeUserRoleDto dto)
    {
        var response = await _adminService.ChangeUserRoleAsync(id, dto);

        if (response.Errors is { Count: > 0 })
            return NotFound(response);

        return Ok(response);
    }

    [HttpDelete("users/{id}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUserAsync(int id)
    {
        var response = await _adminService.DeleteUserAsync(id);

        if (response.Errors is { Count: > 0 })
            return NotFound(response);

        return Ok(response);
    }
    
    [HttpPatch("users/{id}/approve")]
    public async Task<ActionResult<ApiResponseDto<AdminUserDto>>> ApproveUserAsync(
        int id)
    {
        var response = await _adminService.ApproveUserAsync(id);

        if (response.Errors is { Count: > 0 })
            return NotFound(response);

        return Ok(response);
    }
    [HttpPost("users/{id}/reset-password")]
    public async Task<ActionResult<ApiResponseDto<bool>>> ResetUserPasswordAsync(
        int id)
    {
        var response =
            await _passwordResetService.RequestPasswordResetAsync(id);

        if (response.Errors is { Count: > 0 })
            return NotFound(response);

        return Ok(response);
    }
}
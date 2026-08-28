using EnrollmentManager.API.DTOS;
using EnrollmentManager.API.DTOS.Auth;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponseDto<string>>> Register([FromBody] RegisterUserDto dto)
    {
        var response = await _authService.RegisterAsync(dto);

        if (response.Errors is { Count: > 0 })
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseDto<string>>> Login([FromBody] LoginUserDto dto)
    {
        var response = await _authService.LoginAsync(dto);

        if (response.Errors is { Count: > 0 })
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponseDto<bool>>> ForgotPassword(
        [FromServices] IPasswordResetService passwordResetService,
        [FromBody] ForgotPasswordDto dto)
    {
        var response = await passwordResetService.RequestPasswordResetAsync(dto.Email);
        return response.Errors is { Count: > 0 } ? BadRequest(response) : Ok(response);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponseDto<bool>>> ResetPassword(
        [FromServices] IPasswordResetService passwordResetService,
        [FromBody] ResetPasswordDto dto)
    {
        var response = await passwordResetService.ResetPasswordAsync(dto);
        return response.Errors is { Count: > 0 } ? BadRequest(response) : Ok(response);
    }
}

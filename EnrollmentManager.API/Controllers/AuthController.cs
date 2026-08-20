using EnrollmentManager.API.DTOS;
using EnrollmentManager.API.DTOS.Auth;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
}
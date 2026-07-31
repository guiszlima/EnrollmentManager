using EnrollmentManager.API.DTOS;
using EnrollmentManager.API.DTOS.Auth;
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
    public async Task<ActionResult<ApiResponseDTO<string>>> Register([FromBody] RegisterUserDTO dto)
    {
        var response = await _authService.RegisterAsync(dto);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseDTO<string>>> Login([FromBody] LoginUserDTO dto)
    {
        var response = await _authService.LoginAsync(dto);

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }
}
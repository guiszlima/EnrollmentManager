using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Teacher;
using EnrollmentManager.API.DTOs.Auth;
using EnrollmentManager.API.Services.Interfaces.Auth;
using EnrollmentManager.API.Services.Interfaces.Teacher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/teachers")]
[Authorize]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _service;
    private readonly IAuthService _authService;

    public TeacherController(ITeacherService service, IAuthService authService)
    {
        _service = service;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<TeacherResponseDto>>>> GetAll([FromQuery] TeacherFilterDto filter)
    {
        if (!User.IsInRole("Admin") && !User.IsInRole("Secretary"))
            return Forbid();

        return Ok(new ApiResponseDto<List<TeacherResponseDto>> { Data = await _service.GetAllAsync(filter) });
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<ApiResponseDto<TeacherResponseDto>>> GetById(int userId)
    {
        var teacher = await _service.GetByIdAsync(userId);
        return teacher is null
            ? NotFound(ApiResponseDto<TeacherResponseDto>.Error("Professor não encontrado."))
            : Ok(new ApiResponseDto<TeacherResponseDto> { Data = teacher });
    }

    [HttpPost("with-user")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<TeacherResponseDto>>> CreateUserWithTeacher(
        UserTeacherCreateDto dto)
    {
        var userResponse = await _authService.RegisterTeacherAsync(
            new RegisterUserDto(dto.UserName, dto.Email, dto.Password));

        if (userResponse.Errors is { Count: > 0 })
            return BadRequest(userResponse);

        var response = await _service.CreateAsync(userResponse.Data, dto.FormatIds);
        if (response.Data is null)
            return BadRequest(response);

        return CreatedAtAction(nameof(GetById), new { userId = response.Data.UserId }, response);
    }

    [HttpPut("{userId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<TeacherResponseDto>>> Update(
        int userId,
        TeacherConfigureFormatsDto dto)
    {
        var response = await _service.UpdateAsync(userId, dto.FormatIds);
        return response.Data is null ? BadRequest(response) : Ok(response);
    }
}

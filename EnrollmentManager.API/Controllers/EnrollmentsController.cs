using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Enrollment;
using EnrollmentManager.API.Services.Interfaces.Enrollment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<EnrollmentResponseDTO>>> Create(EnrollmentCreateDTO dto)
    {
        if (!CanAccessStudent(dto.StudentId))
            return Forbid();

        var response = await _service.CreateAsync(dto);
        return response.Errors is { Count: > 0 }
            ? BadRequest(response)
            : CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<EnrollmentResponseDTO>>> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        if (response.Errors is { Count: > 0 })
            return NotFound(response);
        if (!CanAccessStudent(response.Data!.StudentId))
            return Forbid();
        return Ok(response);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<ApiResponseDto<List<EnrollmentResponseDTO>>>> GetByStudent(int studentId)
    {
        if (!CanAccessStudent(studentId))
            return Forbid();
        return Ok(await _service.GetByStudentAsync(studentId));
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<EnrollmentResponseDTO>>> ChangeStatus(int id, EnrollmentStatusChangeDto dto)
    {
        var response = await _service.ChangeStatusAsync(id, dto);
        return response.Errors is { Count: > 0 } ? BadRequest(response) : Ok(response);
    }

    private bool CanAccessStudent(int studentId) => User.IsInRole("Admin") ||
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) && userId == studentId;
}

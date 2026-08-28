using EnrollmentManager.API.Constants;
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

    public EnrollmentsController(IEnrollmentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<EnrollmentResponseDTO>>> Create(
        EnrollmentCreateDTO dto,
        CancellationToken cancellationToken)
    {
        if (!CanAccessStudent(dto.StudentId))
            return Forbid();

        var response = await _service.CreateAsync(
            dto,
            cancellationToken);

        if (response.Errors is { Count: > 0 })
            return BadRequest(response);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Data!.Id },
            response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<EnrollmentResponseDTO>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _service.GetByIdAsync(
            id,
            cancellationToken);

        if (response.Errors is { Count: > 0 })
            return NotFound(response);

        if (!CanAccessStudent(response.Data!.StudentId))
            return Forbid();

        return Ok(response);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<ApiResponseDto<List<EnrollmentResponseDTO>>>> GetByStudent(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!CanAccessStudent(studentId))
            return Forbid();

        var response = await _service.GetByStudentAsync(
            studentId,
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<ActionResult<ApiResponseDto<EnrollmentResponseDTO>>> ChangeStatus(
        int id,
        EnrollmentStatusChangeDto dto,
        CancellationToken cancellationToken)
    {
        var response = await _service.ChangeStatusAsync(
            id,
            dto,
            cancellationToken);

        if (response.Errors is { Count: > 0 })
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Permite acesso se o usuário for Staff
    /// ou se for o próprio aluno dono do recurso.
    /// </summary>
    private bool CanAccessStudent(int studentId) =>
        IsStaff() ||
        (
            int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId)
            && userId == studentId
        );

    private bool IsStaff() =>
        User.IsInRole(Roles.Admin) ||
        User.IsInRole(Roles.Secretary);
}
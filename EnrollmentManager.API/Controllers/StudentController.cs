using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOS.Student;
using EnrollmentManager.API.Services.Interfaces.Student;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<StudentResponseDTO>>>> GetAll()
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        return Ok(new ApiResponseDto<List<StudentResponseDTO>>
        {
            Data = await _service.GetAllAsync()
        });
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<ApiResponseDto<StudentResponseDTO>>> GetById(int userId)
    {
        if (!CanAccessStudent(userId))
            return Forbid();

        var student = await _service.GetByIdAsync(userId);

        if (student is null)
            return NotFound(ApiResponseDto<StudentResponseDTO>.Error("Aluno não encontrado."));

        return Ok(new ApiResponseDto<StudentResponseDTO> { Data = student });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<StudentResponseDTO>>> Create(StudentCreateDTO dto)
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        var student = await _service.CreateAsync(dto);

        if (student is null)
            return Conflict(ApiResponseDto<StudentResponseDTO>.Error(
                "Usuário inexistente, aluno já cadastrado ou dados duplicados."));

        return CreatedAtAction(
            nameof(GetById),
            new { userId = student.UserId },
            new ApiResponseDto<StudentResponseDTO>
            {
                Data = student,
                Message = "Aluno cadastrado com sucesso."
            });
    }

    [HttpPut("{userId:int}")]
    public async Task<ActionResult<ApiResponseDto<StudentResponseDTO>>> Update(
        int userId,
        StudentUpdateDTO dto)
    {
        if (!CanAccessStudent(userId))
            return Forbid();

        var student = await _service.UpdateAsync(userId, dto);

        if (student is null)
        {
            var existing = await _service.GetByIdAsync(userId);

            if (existing is null)
                return NotFound(ApiResponseDto<StudentResponseDTO>.Error("Aluno não encontrado."));

            return Conflict(ApiResponseDto<StudentResponseDTO>.Error("Já existe um aluno com esses dados."));
        }

        return Ok(new ApiResponseDto<StudentResponseDTO>
        {
            Data = student,
            Message = "Aluno atualizado com sucesso."
        });
    }

    [HttpDelete("{userId:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(int userId)
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        if (!await _service.DeleteAsync(userId))
            return NotFound(ApiResponseDto<bool>.Error("Aluno não encontrado."));

        return Ok(new ApiResponseDto<bool>
        {
            Data = true,
            Message = "Aluno removido com sucesso."
        });
    }

    private bool CanAccessStudent(int userId) =>
        User.IsInRole("Admin") ||
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authenticatedUserId) &&
        authenticatedUserId == userId;
}

using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Student;
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
    public async Task<ActionResult<ApiResponseDto<List<StudentResponseDto>>>> GetAll()
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        return Ok(new ApiResponseDto<List<StudentResponseDto>>
        {
            Data = await _service.GetAllAsync()
        });
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<ApiResponseDto<StudentResponseDto>>> GetById(int userId)
    {
        if (!CanAccessStudent(userId))
            return Forbid();

        var student = await _service.GetByIdAsync(userId);

        if (student is null)
            return NotFound(ApiResponseDto<StudentResponseDto>.Error("Aluno não encontrado."));

        return Ok(new ApiResponseDto<StudentResponseDto> { Data = student });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<StudentResponseDto>>> Create(StudentCreateDto dto)
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        var response = await _service.CreateAsync(dto);

        // Se Data vier nulo (ou !response.Success caso você tenha a propriedade booleana)
        if (response.Data is null)
            return BadRequest(response); // Retorna 400 com a mensagem formatada pelo Serviço

        return CreatedAtAction(
            nameof(GetById),
            new { userId = response.Data.UserId },
            response);
    }

    [HttpPut("{userId:int}")]
    public async Task<ActionResult<ApiResponseDto<StudentResponseDto>>> Update(
        int userId,
        StudentUpdateDto dto)
    {
        if (!CanAccessStudent(userId))
            return Forbid();

        var response = await _service.UpdateAsync(userId, dto);

        if (response.Data is null)
        {
            // Se a mensagem indicar que não encontrou, devolvemos 404. Caso contrário, 400.
            if (response.Message?.Contains("não encontrado") == true)
                return NotFound(response);
            
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{userId:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(int userId)
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        var response = await _service.DeleteAsync(userId);

        if (response.Data is false) // Falhou na validação de exclusão ou não encontrado
        {
            if (response.Message?.Contains("não encontrado") == true)
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    private bool CanAccessStudent(int userId) =>
        User.IsInRole("Admin") ||
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authenticatedUserId) &&
        authenticatedUserId == userId;
}
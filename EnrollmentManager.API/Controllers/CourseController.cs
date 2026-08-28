using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Course;
using EnrollmentManager.API.Services.Interfaces.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly ICourseService _service;

    public CourseController(ICourseService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<CourseResponseDTO>>>> GetAll() =>
        Ok(new ApiResponseDto<List<CourseResponseDTO>> { Data = await _service.GetAllAsync() });

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<CourseResponseDTO>>> GetById(int id)
    {
        var course = await _service.GetByIdAsync(id);
        return course is null
            ? NotFound(ApiResponseDto<CourseResponseDTO>.Error("Curso não encontrado."))
            : Ok(new ApiResponseDto<CourseResponseDTO> { Data = course });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<CourseResponseDTO>>> Create(CourseInputDTO dto)
    {
        var course = await _service.CreateAsync(dto);
        return course is null
            ? BadRequest(ApiResponseDto<CourseResponseDTO>.Error("Classificações do curso inválidas."))
            : CreatedAtAction(nameof(GetById), new { id = course.Id }, new ApiResponseDto<CourseResponseDTO> { Data = course });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<CourseResponseDTO>>> Update(int id, CourseInputDTO dto)
    {
        var course = await _service.UpdateAsync(id, dto);
        return course is null
            ? BadRequest(ApiResponseDto<CourseResponseDTO>.Error("Curso não encontrado ou classificações inválidas."))
            : Ok(new ApiResponseDto<CourseResponseDTO> { Data = course });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(int id)
    {
        if (!await _service.DeleteAsync(id))
            return NotFound(ApiResponseDto<bool>.Error("Curso não encontrado."));
        return Ok(new ApiResponseDto<bool> { Data = true });
    }
}

using EnrollmentManager.API.Constants;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Course;
using EnrollmentManager.API.DTOs.CourseReport;
using EnrollmentManager.API.Services.Interfaces.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnrollmentManager.API.Services.Courses;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly ICourseService _service;
    private readonly ICourseReportService _reportService;

    public CourseController(ICourseService service, ICourseReportService reportService)
    {
        _service = service;
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<CourseResponseDto>>>> GetAll([FromQuery] CourseFilterDto filter) =>
        Ok(new ApiResponseDto<List<CourseResponseDto>> { Data = await _service.GetAllAsync(filter) });

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<CourseResponseDto>>> GetById(int id)
    {
        var course = await _service.GetByIdAsync(id);
        return course is null
            ? NotFound(ApiResponseDto<CourseResponseDto>.Error("Curso não encontrado."))
            : Ok(new ApiResponseDto<CourseResponseDto> { Data = course });
    }

    [HttpGet("{id:int}/report")]
    public async Task<ActionResult<ApiResponseDto<CourseReportDto>>> Report(
        int id,
        [FromQuery] CourseReportFilterDto filter)
    {
        var report = await _reportService.GetAsync(id, filter);
        return report is null
            ? NotFound(ApiResponseDto<CourseReportDto>.Error("Curso não encontrado."))
            : Ok(new ApiResponseDto<CourseReportDto> { Data = report });
    }

    [HttpPost]
    [Authorize(Roles = Roles.Staff)]
    public async Task<ActionResult<ApiResponseDto<CourseResponseDto>>> Create(CourseInputDto dto)
    {
        try
        {
            var course = await _service.CreateAsync(dto);
            return course is null
                ? BadRequest(ApiResponseDto<CourseResponseDto>.Error("Não foi possível criar o curso."))
                : CreatedAtAction(nameof(GetById), new { id = course.Id }, new ApiResponseDto<CourseResponseDto> { Data = course });
        }
        catch (CourseValidationException exception)
        {
            return BadRequest(ApiResponseDto<CourseResponseDto>.Error(exception.Message));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<CourseResponseDto>>> Update(int id, CourseInputDto dto)
    {
        try
        {
            var course = await _service.UpdateAsync(id, dto);
            return course is null
                ? BadRequest(ApiResponseDto<CourseResponseDto>.Error("Curso não encontrado."))
                : Ok(new ApiResponseDto<CourseResponseDto> { Data = course });
        }
        catch (CourseValidationException exception)
        {
            return BadRequest(ApiResponseDto<CourseResponseDto>.Error(exception.Message));
        }
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

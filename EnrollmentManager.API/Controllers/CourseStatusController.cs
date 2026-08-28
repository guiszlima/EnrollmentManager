using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Services.Interfaces.Course;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/course-statuses")]
[Authorize]
public class CourseStatusController : ControllerBase
{
    private readonly ICourseStatusService _service;

    public CourseStatusController(ICourseStatusService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<CourseStatusResponseDto>>>> GetAll()
    {
        return Ok(new ApiResponseDto<List<CourseStatusResponseDto>>
        {
            Data = await _service.GetAllAsync()
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<CourseStatusResponseDto>>> GetById(int id)
    {
        var courseStatus = await _service.GetByIdAsync(id);

        if (courseStatus is null)
            return NotFound(ApiResponseDto<CourseStatusResponseDto>.Error("Status do curso não encontrado."));

        return Ok(new ApiResponseDto<CourseStatusResponseDto> { Data = courseStatus });
    }
}

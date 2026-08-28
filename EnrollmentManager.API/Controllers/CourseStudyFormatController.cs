using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/course-study-formats")]
[Authorize]
public class CourseStudyFormatController : ControllerBase
{
    private readonly ICourseStudyFormatService _service;

    public CourseStudyFormatController(ICourseStudyFormatService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<CourseStudyFormatResponseDto>>>> GetAll() =>
        Ok(new ApiResponseDto<List<CourseStudyFormatResponseDto>> { Data = await _service.GetAllAsync() });

    [HttpGet("{courseId:int}/{formatId:int}")]
    public async Task<ActionResult<ApiResponseDto<CourseStudyFormatResponseDto>>> GetById(
        int courseId,
        int formatId)
    {
        var result = await _service.GetByIdAsync(courseId, formatId);
        return result is null
            ? NotFound(ApiResponseDto<CourseStudyFormatResponseDto>.Error("Formato do curso não encontrado."))
            : Ok(new ApiResponseDto<CourseStudyFormatResponseDto> { Data = result });
    }
}

using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/course-types")]
[Authorize]
public class CourseTypeController : ControllerBase
{
    private readonly ICourseTypeService _service;

    public CourseTypeController(ICourseTypeService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<CourseTypeResponseDto>>>> GetAll() =>
        Ok(new ApiResponseDto<List<CourseTypeResponseDto>> { Data = await _service.GetAllAsync() });

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<CourseTypeResponseDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null
            ? NotFound(ApiResponseDto<CourseTypeResponseDto>.Error("Tipo de curso não encontrado."))
            : Ok(new ApiResponseDto<CourseTypeResponseDto> { Data = result });
    }
}

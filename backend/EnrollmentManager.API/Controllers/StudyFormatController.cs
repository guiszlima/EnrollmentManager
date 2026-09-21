

using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.StudyFormat;
using EnrollmentManager.API.Services.Interfaces.StudyFormat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/study-formats")]
[Authorize]
public class StudyFormatController : ControllerBase
{
    private readonly IStudyFormatService _service;

    public StudyFormatController(IStudyFormatService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<StudyFormatDto>>>> GetAll()
    {
        var response = await _service.GetAllAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<StudyFormatDto>>> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);

        if (response.Data is null)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}
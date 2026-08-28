using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/education-levels")]
[Authorize]
public class EducationLevelController : ControllerBase
{
    private readonly IEducationLevelService _service;

    public EducationLevelController(IEducationLevelService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<EducationLevelResponseDto>>>> GetAll() =>
        Ok(new ApiResponseDto<List<EducationLevelResponseDto>> { Data = await _service.GetAllAsync() });

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<EducationLevelResponseDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null
            ? NotFound(ApiResponseDto<EducationLevelResponseDto>.Error("Nível de educação não encontrado."))
            : Ok(new ApiResponseDto<EducationLevelResponseDto> { Data = result });
    }
}

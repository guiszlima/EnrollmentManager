using EnrollmentManager.API.DTOs.Catalogs;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.Services.Interfaces.Catalogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/enrollment-statuses")]
[Authorize]
public class EnrollmentStatusController : ControllerBase
{
    private readonly IEnrollmentStatusService _service;

    public EnrollmentStatusController(IEnrollmentStatusService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<EnrollmentStatusResponseDto>>>> GetAll() =>
        Ok(new ApiResponseDto<List<EnrollmentStatusResponseDto>> { Data = await _service.GetAllAsync() });

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<EnrollmentStatusResponseDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null
            ? NotFound(ApiResponseDto<EnrollmentStatusResponseDto>.Error("Status de matrícula não encontrado."))
            : Ok(new ApiResponseDto<EnrollmentStatusResponseDto> { Data = result });
    }
}

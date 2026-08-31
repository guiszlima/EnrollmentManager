using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.Role;
using EnrollmentManager.API.Services.Interfaces.Role;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = "Admin")]
public class RoleController : ControllerBase
{
	private readonly IRoleService _service;

	public RoleController(IRoleService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<ActionResult<ApiResponseDto<List<RoleResponseDto>>>> GetAll()
	{
		return Ok(new ApiResponseDto<List<RoleResponseDto>>
		{
			Data = await _service.GetAllAsync()
		});
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<ApiResponseDto<RoleResponseDto>>> GetById(int id)
	{
		var role = await _service.GetByIdAsync(id);

		if (role is null)
			return NotFound(ApiResponseDto<RoleResponseDto>.Error("Role não encontrada."));

		return Ok(new ApiResponseDto<RoleResponseDto> { Data = role });
	}
}

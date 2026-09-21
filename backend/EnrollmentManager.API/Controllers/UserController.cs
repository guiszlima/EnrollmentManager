using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("available-for-student")]
    public async Task<ActionResult<ApiResponseDto<List<UserResponseDto>>>> GetAvailableForStudentAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Where(user =>
                user.IsActive &&
                user.Role != null &&
                user.Role.Code == "STUDENT" &&
                user.Student == null)
            .OrderBy(user => user.UserName)
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            })
            .ToListAsync(cancellationToken);

        return Ok(new ApiResponseDto<List<UserResponseDto>>
        {
            Data = users
        });
    }
}
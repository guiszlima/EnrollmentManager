using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.StudyFormat;
using EnrollmentManager.API.Services.Interfaces.StudyFormat;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.StudyFormat;

public class StudyFormatService : IStudyFormatService
{
    private readonly ApplicationDbContext _context;

    public StudyFormatService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponseDto<List<StudyFormatDto>>> GetAllAsync()
    {
        var formats = await _context.StudyFormats
            .AsNoTracking()
            .Select(x => new StudyFormatDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return new ApiResponseDto<List<StudyFormatDto>>
        {
            Data = formats
        };
    }

    public async Task<ApiResponseDto<StudyFormatDto>> GetByIdAsync(int id)
    {
        var format = await _context.StudyFormats
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new StudyFormatDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync();

        if (format is null)
        {
            return ApiResponseDto<StudyFormatDto>.Error(
                "Formato de estudo não encontrado.");
        }

        return new ApiResponseDto<StudyFormatDto>
        {
            Data = format
        };
    }
}
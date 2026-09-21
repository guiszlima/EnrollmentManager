using EnrollmentManager.API.DTOs.Common;
using EnrollmentManager.API.DTOs.StudyFormat;

namespace EnrollmentManager.API.Services.Interfaces.StudyFormat;

public interface IStudyFormatService
{
    Task<ApiResponseDto<List<StudyFormatDto>>> GetAllAsync();

    Task<ApiResponseDto<StudyFormatDto>> GetByIdAsync(int id);
}
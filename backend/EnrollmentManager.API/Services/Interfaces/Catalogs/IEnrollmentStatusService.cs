using EnrollmentManager.API.DTOs.Catalogs;

namespace EnrollmentManager.API.Services.Interfaces.Catalogs;

public interface IEnrollmentStatusService
{
    Task<List<EnrollmentStatusResponseDto>> GetAllAsync();
    Task<EnrollmentStatusResponseDto?> GetByIdAsync(int id);
}
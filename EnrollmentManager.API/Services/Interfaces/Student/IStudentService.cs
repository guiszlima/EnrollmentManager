using EnrollmentManager.API.DTOS.Student;

namespace EnrollmentManager.API.Services.Interfaces.Student;

public interface IStudentService
{
    Task<List<StudentResponseDTO>> GetAllAsync();

    Task<StudentResponseDTO?> GetByIdAsync(int userId);

    Task<StudentResponseDTO?> CreateAsync(StudentCreateDTO dto);

    Task<StudentResponseDTO?> UpdateAsync(int userId, StudentUpdateDTO dto);

    Task<bool> DeleteAsync(int userId);
}
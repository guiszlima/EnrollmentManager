using EnrollmentManager.API.DTOs.Course;
using EnrollmentManager.API.DTOs.CourseReport;

namespace EnrollmentManager.API.Services.Interfaces.Course;

public interface ICourseReportService
{
    Task<CourseReportDto?> GetAsync(int courseId, CourseReportFilterDto filter);
}

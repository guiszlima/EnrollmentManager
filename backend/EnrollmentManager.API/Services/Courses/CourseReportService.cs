using EnrollmentManager.API.Data;
using EnrollmentManager.API.DTOs.Course;
using EnrollmentManager.API.DTOs.CourseReport;
using EnrollmentManager.API.Services.Interfaces.Course;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Services.Courses;

public class CourseReportService : ICourseReportService
{
    private readonly ApplicationDbContext _context;

    public CourseReportService(ApplicationDbContext context) => _context = context;

    public async Task<CourseReportDto?> GetAsync(int courseId, CourseReportFilterDto filter)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Include(current => current.CourseType)
            .Include(current => current.EducationLevel)
            .Include(current => current.CourseStatus)
            .Include(current => current.AllowedFormats)
                .ThenInclude(format => format.Format)
            .Include(current => current.Enrollments)
                .ThenInclude(enrollment => enrollment.Student)
                    .ThenInclude(student => student.User)
            .Include(current => current.Enrollments)
                .ThenInclude(enrollment => enrollment.Status)
            .Include(current => current.Enrollments)
                .ThenInclude(enrollment => enrollment.Format)
            .Include(current => current.CourseTeachers)
                .ThenInclude(courseTeacher => courseTeacher.Teacher)
                    .ThenInclude(teacher => teacher.User)
            .Include(current => current.CourseTeachers)
                .ThenInclude(courseTeacher => courseTeacher.Teacher)
                    .ThenInclude(teacher => teacher.AllowedFormats)
                        .ThenInclude(format => format.Format)
            .FirstOrDefaultAsync(current => current.Id == courseId);

        if (course is null)
            return null;

        var enrollmentQuery = course.Enrollments.AsEnumerable();
        if (filter.EnrollmentStatusIds.Count > 0)
            enrollmentQuery = enrollmentQuery.Where(enrollment => filter.EnrollmentStatusIds.Contains(enrollment.StatusId));
        if (filter.FormatIds.Count > 0)
            enrollmentQuery = enrollmentQuery.Where(enrollment => filter.FormatIds.Contains(enrollment.FormatId));

        var teacherQuery = course.CourseTeachers.AsEnumerable();
        if (filter.TeacherIds.Count > 0)
            teacherQuery = teacherQuery.Where(courseTeacher => filter.TeacherIds.Contains(courseTeacher.TeacherId));

        return new CourseReportDto
        {
            Course = new CourseReportCourseDto
            {
                Id = course.Id,
                Name = course.Name,
                CourseTypeName = course.CourseType.Name,
                EducationLevelName = course.EducationLevel.Name,
                StatusName = course.CourseStatus.Name,
                TotalSlots = course.TotalSlots,
                AvailableSlots = course.AvailableSlots,
                EnrollmentCount = course.Enrollments.Count,
                Formats = course.AllowedFormats.Select(format => MapFormat(format.Format)).ToList()
            },
            Enrollments = enrollmentQuery.Select(enrollment => new CourseReportEnrollmentDto
            {
                Id = enrollment.Id,
                EnrollmentStatus = enrollment.Status.Name,
                EnrollmentDate = enrollment.EnrollmentDate,
                Format = enrollment.Format is null ? null : MapFormat(enrollment.Format),
                Student = new CourseReportStudentDto
                {
                    UserId = enrollment.Student.UserId,
                    Name = enrollment.Student.User.UserName,
                    Email = enrollment.Student.User.Email,
                    RegistrationNumber = enrollment.Student.RegistrationNumber ?? string.Empty,
                    Nationality = enrollment.Student.Nationality,
                    Phone = enrollment.Student.Phone
                }
            }).ToList(),
            Teachers = teacherQuery.Select(courseTeacher => new CourseReportTeacherDto
            {
                UserId = courseTeacher.Teacher.UserId,
                Name = courseTeacher.Teacher.User.UserName,
                Email = courseTeacher.Teacher.User.Email,
                Formats = courseTeacher.Teacher.AllowedFormats.Select(format => MapFormat(format.Format)).ToList()
            }).ToList()
        };
    }

    private static CourseReportFormatDto MapFormat(EnrollmentManager.API.Models.StudyFormat format) => new()
    {
        Id = format.Id,
        Name = format.Name
    };
}

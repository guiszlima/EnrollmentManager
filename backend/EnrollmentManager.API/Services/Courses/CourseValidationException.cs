namespace EnrollmentManager.API.Services.Courses;

public sealed class CourseValidationException(string message) : Exception(message);

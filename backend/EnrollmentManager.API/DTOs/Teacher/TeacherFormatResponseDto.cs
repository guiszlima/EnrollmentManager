
using EnrollmentManager.API.DTOs.StudyFormat;

namespace EnrollmentManager.API.DTOs.Teacher
{
    

    public class TeacherFormatResponseDto
    {
        public int TeacherId { get; set; }
        public List<StudyFormatDto> Formats { get; set; } = new();
    }
}
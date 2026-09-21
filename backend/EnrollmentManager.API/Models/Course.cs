using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentManager.API.Models
{
    public class Course
    {
        
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CourseTypeId { get; set; }

        [ForeignKey(nameof(CourseTypeId))]
        public CourseType CourseType { get; set; } = null!;

        [Required]
        public int EducationLevelId { get; set; }

        [ForeignKey(nameof(EducationLevelId))]
        public EducationLevel EducationLevel { get; set; } = null!;

        [Required]
        public int CourseStatusId { get; set; }

        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }

        [ForeignKey(nameof(CourseStatusId))]
        public CourseStatus CourseStatus { get; set; } = null!;

        public ICollection<CourseStudyFormat> AllowedFormats { get; set; }
            = new List<CourseStudyFormat>();

        public ICollection<CourseTeacher> CourseTeachers { get; set; }
            = new List<CourseTeacher>();

        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();
    }
}

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

        [ForeignKey(nameof(CourseStatusId))]
        public CourseStatus CourseStatus { get; set; } = null!;

        // Formatos em que este curso está disponível (via tabela de junção)
        public ICollection<CourseStudyFormat> Format { get; set; } = new List<CourseStudyFormat>();
    }
}
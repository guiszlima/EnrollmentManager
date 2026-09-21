using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentManager.API.Models
{
    public class TeacherStudyFormat
    {
        [Required]
        public int TeacherId { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; } = null!;

        [Required]
        public int FormatId { get; set; }

        [ForeignKey(nameof(FormatId))]
        public StudyFormat Format { get; set; } = null!;
    }
}
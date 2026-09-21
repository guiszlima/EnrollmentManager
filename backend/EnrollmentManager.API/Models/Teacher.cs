using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentManager.API.Models
{
    public class Teacher
    {
        [Key, ForeignKey(nameof(User))]
    public int UserId { get; set; }

        public User User { get; set; } = null!;

        public ICollection<TeacherStudyFormat> AllowedFormats { get; set; }
            = new List<TeacherStudyFormat>();
    }
}
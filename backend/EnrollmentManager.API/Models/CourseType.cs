using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.Models;

public class CourseType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}

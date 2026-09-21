using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentManager.API.Models;

public class CourseTeacher
{
    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    public int TeacherId { get; set; }

    [ForeignKey(nameof(TeacherId))]
    public Teacher Teacher { get; set; } = null!;
}
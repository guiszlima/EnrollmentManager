using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentManager.API.Models;

public class CourseStudyFormat
{
    [Required]
    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))] 
    public Course Course { get; set; } = null!;
    [Required]
    public int FormatId { get; set; }

    [ForeignKey(nameof(FormatId))]
    public  StudyFormat Format { get; set; } = null!;

}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentManager.API.Models
{[Index(nameof(Code), IsUnique = true)]
    public class CourseStatus
    {
    [Key]  
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Description { get; set; }
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
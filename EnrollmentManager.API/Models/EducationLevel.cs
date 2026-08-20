using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.Models;

public class EducationLevel
{
    [Key] public int Id { get; set; }

    [Required] 
    [MaxLength(50)] 
    public string Name { get; set; } = string.Empty; // Ex: Undergraduate, Technical, Postgraduate, etc.

    // Propriedade de navegação: Uma categoria de programa pode estar em várias matrículas
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
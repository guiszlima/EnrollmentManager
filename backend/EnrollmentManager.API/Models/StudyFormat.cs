using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.Models
{
    public class StudyFormat
    {
        [Key] 
        public int Id { get; set; }

        [Required] 
        [MaxLength(50)] 
        public string Name { get; set; } = string.Empty; // Ex: Presential, EAD, Hybrid, International

        // Propriedade de navegação: Uma modalidade pode estar associada a várias matrículas
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
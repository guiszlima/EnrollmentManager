using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentManager.API.Models;

public class Student
{
    [Key, ForeignKey("User")]
    public int UserId { get; set; }

    // O CPF passa a ser opcional (string?) caso seja um aluno internacional
    [MaxLength(14)] 
    public string? Cpf { get; set; }

    // Campos para alunos internacionais
    [MaxLength(50)] 
    public string? PassportNumber { get; set; }

    [MaxLength(50)] 
    public string? Nationality { get; set; }

    public DateTime BirthDate { get; set; }
    
    [MaxLength(20)] 
    public string Phone { get; set; } = string.Empty;
    
    [MaxLength(200)] 
    public string Address { get; set; } = string.Empty;
    
    [Required] 
    [MaxLength(50)] 
    public string RegistrationNumber { get; set; } = string.Empty;

    // Propriedade de navegação
    public User User { get; set; } = null!;
}
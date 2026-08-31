using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.DTOs.Student;

public class StudentUpdateDto
{
    [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter entre 11 e 14 caracteres.")]
    public string? Cpf { get; init; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "O passaporte deve ter entre 2 e 50 caracteres.")]
    public string? PassportNumber { get; init; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "A nacionalidade deve ter entre 2 e 50 caracteres.")]
    public string? Nationality { get; init; }

    [DataType(DataType.Date)]
    [Range(typeof(DateTime), "1900-01-01", "2100-12-31", ErrorMessage = "A data de nascimento é inválida.")]
    public DateTime BirthDate { get; init; }

    [Required, StringLength(20, MinimumLength = 8, ErrorMessage = "O telefone deve ter entre 8 e 20 caracteres.")]
    public string Phone { get; init; } = string.Empty;

    [Required, StringLength(200, MinimumLength = 5, ErrorMessage = "O endereço deve ter entre 5 e 200 caracteres.")]
    public string Address { get; init; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 2, ErrorMessage = "O número de matrícula deve ter entre 2 e 50 caracteres.")]
    public string RegistrationNumber { get; init; } = string.Empty;
}

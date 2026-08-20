using System.ComponentModel.DataAnnotations;

namespace EnrollmentManager.API.Dtos.User;

public record ChangeUserRoleDto
{
    [Required(ErrorMessage = "O cargo é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O cargo informado é inválido.")]
    public int RoleId { get; init; }
}
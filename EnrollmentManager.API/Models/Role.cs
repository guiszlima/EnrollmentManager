using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EnrollmentManager.API.Models;

public class Role
{
    [Key] public int Id { get; set; }

    [Required] [MaxLength(50)] public string Name { get; set; } = string.Empty;

    // Propriedade de navegação para a relação 1:N
    public ICollection<User> Users { get; set; } = new List<User>();
}
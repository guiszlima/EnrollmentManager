using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentManager.API.DTOs.Admin
{
    public class ApproveUserDto
    {
        [Required(ErrorMessage = "O ID do papel é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do papel deve ser maior que zero.")]
        public int RoleId { get; set; }
    }
}

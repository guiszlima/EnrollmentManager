using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EnrollmentManager.API.DTOs.Student
{
    public class UserStudentCreateDto
    {
        [Required, StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        private string? _cpf;
        [MaxLength(14)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos.")]
        public string? Cpf
        {
            get => _cpf;
            set => _cpf = CleanCpf(value);
        }

        [MaxLength(50)]
        public string? PassportNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nationality { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "É necessário informar pelo menos um formato de estudo.")]
        public List<int> FormatIds { get; set; } = new();

        private static string? CleanCpf(string? rawCpf)
        {
            if (string.IsNullOrWhiteSpace(rawCpf))
                return null;

            var onlyNumbers = Regex.Replace(rawCpf, @"[^\d]", "");
            return string.IsNullOrWhiteSpace(onlyNumbers) ? null : onlyNumbers;
        }
    }
}

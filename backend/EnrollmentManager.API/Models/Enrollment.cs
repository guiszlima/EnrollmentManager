using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentManager.API.Models;

public class Enrollment
{
    [Key]
    public int Id { get; set; }

    // --- ALUNO ---
    [Required]
    public int StudentId { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;

    // --- CURSO (Substitui ProgramName, CourseTypeId e EducationLevelId) ---
    [Required]
    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    // --- STATUS DA MATRÍCULA ---
    [Required]
    public int StatusId { get; set; }

    [ForeignKey(nameof(StatusId))]
    public EnrollmentStatus Status { get; set; } = null!;

    // --- FORMATO / MODALIDADE (Alinhado com EnrollmentFormat) ---
    [Required]
    public int FormatId { get; set; }

    [ForeignKey(nameof(FormatId))]
    public StudyFormat Format { get; set; } = null!;

    // --- DATAS ---
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public DateTime? CompletionDate { get; set; }

    // Mantido pelo serviço a partir do código do status. Permite ao PostgreSQL
    // garantir que não existam matrículas simultaneamente ativas.
    public bool IsActiveEnrollment { get; set; }

    public bool ConsumesSeat { get; set; } = true;
    
    
    public void ApplyStatusChange(EnrollmentStatus targetStatus, string approvedCode, string completedCode)
    {
        StatusId = targetStatus.Id;
        Status = targetStatus;
        IsActiveEnrollment = targetStatus.Code == approvedCode;
        CompletionDate = targetStatus.Code == completedCode ? DateTime.UtcNow : null;
    }
}

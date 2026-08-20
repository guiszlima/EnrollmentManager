using Microsoft.EntityFrameworkCore;
using EnrollmentManager.API.Models;

namespace EnrollmentManager.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // --- Autenticação & Usuários ---
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Student> Students => Set<Student>();

    // --- Cursos ---
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseType> CourseTypes => Set<CourseType>();
    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();
    public DbSet<CourseStatus> CourseStatuses => Set<CourseStatus>(); // <-- FALTAVA ESTE!
    
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    // --- Formatos / Modalidades de Estudo ---
    public DbSet<StudyFormat> StudyFormats => Set<StudyFormat>(); // Nomes ajustados
    public DbSet<CourseStudyFormat> CourseStudyFormats => Set<CourseStudyFormat>(); // Nomes ajustados

    // --- Matrículas ---
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<EnrollmentStatus> EnrollmentStatuses => Set<EnrollmentStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplica a chave primária composta na tabela de junção
        modelBuilder.Entity<CourseStudyFormat>()
            .HasKey(cf => new { cf.CourseId, cf.FormatId });

        // Aplica automaticamente todas as Fluent Configurations da pasta Configurations (se houver)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
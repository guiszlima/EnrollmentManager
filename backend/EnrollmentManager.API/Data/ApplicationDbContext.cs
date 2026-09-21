using Microsoft.EntityFrameworkCore;
using EnrollmentManager.API.Models;

namespace EnrollmentManager.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // --- Autenticação & Usuários ---
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    // --- Cursos ---
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseType> CourseTypes => Set<CourseType>();
    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();
    public DbSet<CourseStatus> CourseStatuses => Set<CourseStatus>();

    // --- Formatos / Modalidades ---
    public DbSet<StudyFormat> StudyFormats => Set<StudyFormat>();
    public DbSet<CourseStudyFormat> CourseStudyFormats => Set<CourseStudyFormat>();
    public DbSet<StudentStudyFormat> StudentStudyFormats => Set<StudentStudyFormat>();

    // --- Matrículas ---
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<EnrollmentStatus> EnrollmentStatuses => Set<EnrollmentStatus>();

    // --- Professores ---
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<TeacherStudyFormat> TeacherStudyFormats => Set<TeacherStudyFormat>();
    public DbSet<CourseTeacher> CourseTeachers => Set<CourseTeacher>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Course ↔ StudyFormat
        modelBuilder.Entity<CourseStudyFormat>()
            .HasKey(cf => new { cf.CourseId, cf.FormatId });

        // Student - StudyFormat
        modelBuilder.Entity<StudentStudyFormat>()
            .HasKey(sf => new { sf.StudentId, sf.FormatId });

        modelBuilder.Entity<StudentStudyFormat>()
            .HasOne(sf => sf.Student)
            .WithMany(student => student.AllowedFormats)
            .HasForeignKey(sf => sf.StudentId);

        modelBuilder.Entity<StudentStudyFormat>()
            .HasOne(sf => sf.Format)
            .WithMany(format => format.StudentStudyFormats)
            .HasForeignKey(sf => sf.FormatId);

        // Course ↔ Teacher
        modelBuilder.Entity<CourseTeacher>()
            .HasKey(ct => new { ct.CourseId, ct.TeacherId });

        // Teacher - StudyFormat
        modelBuilder.Entity<TeacherStudyFormat>()
            .HasKey(tf => new { tf.TeacherId, tf.FormatId });

        // Garante que o aluno não tenha duas matrículas ATIVAS no mesmo curso,
        // mas permite que ele tenha matrículas inativas/concluídas antigas (preservando histórico).
        modelBuilder.Entity<Enrollment>()
            .HasIndex(enrollment => new { enrollment.StudentId, enrollment.CourseId })
            .IsUnique()
            .HasDatabaseName("IX_Enrollments_StudentId_CourseId_ActiveOnly")
            .HasFilter("\"IsActiveEnrollment\" = TRUE");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
using EnrollmentManager.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrollmentManager.API.Data.Configurations;

public class CourseStatusConfiguration : IEntityTypeConfiguration<CourseStatus>
{
    public void Configure(EntityTypeBuilder<CourseStatus> builder)
    {
        // Garante o índice único pelo código
        builder.HasIndex(c => c.Code)
               .IsUnique();

        // Carga inicial (Seed Data)
        builder.HasData(
            new CourseStatus
            {
                Id = 1,
                Name = "Active",
                Code = "ACTIVE",
                Description = "The course is active and accepting new enrollments."
            },
            new CourseStatus
            {
                Id = 2,
                Name = "Inactive",
                Code = "INACTIVE",
                Description = "The course is inactive and not available for new enrollments."
            },
            new CourseStatus
            {
                Id = 3,
                Name = "Draft",
                Code = "DRAFT",
                Description = "The course is under creation and not yet available to the public."
            },
            new CourseStatus
            {
                Id = 4,
                Name = "Discontinued",
                Code = "DISCONTINUED",
                Description = "The course has been permanently discontinued."
            }
        );
    }
}
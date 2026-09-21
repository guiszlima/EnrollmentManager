using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnrollmentManager.API.Models;

namespace EnrollmentManager.API.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(r => r.Code)
            .IsUnique();

        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                Code = "ADMIN"
            },
            new Role
            {
                Id = 2,
                Name = "Secretary",
                Code = "SECRETARY"
            },
            new Role
            {
                Id = 3,
                Name = "Student",
                Code = "STUDENT"
            },
            new Role
            {
                Id = 4,
                Name = "Teacher",
                Code = "TEACHER"
            }
        );
    }
}
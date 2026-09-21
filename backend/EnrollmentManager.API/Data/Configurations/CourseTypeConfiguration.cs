using EnrollmentManager.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrollmentManager.API.Data.Configurations;

public class CourseTypeConfiguration : IEntityTypeConfiguration<CourseType>
{
    public void Configure(EntityTypeBuilder<CourseType> builder)
    {
        builder.HasData(
            new CourseType
            {
                Id = 1,
                Name = "Bacharelado"
            },
            new CourseType
            {
                Id = 2,
                Name = "Tecnólogo"
            },
            new CourseType
            {
                Id = 3,
                Name = "Licenciatura"
            }
        );
    }
}
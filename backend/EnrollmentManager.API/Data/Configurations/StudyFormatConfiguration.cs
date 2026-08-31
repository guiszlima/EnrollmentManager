using EnrollmentManager.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrollmentManager.API.Data.Configurations;

public class StudyFormatConfiguration
    : IEntityTypeConfiguration<StudyFormat>
{
    public void Configure(EntityTypeBuilder<StudyFormat> builder)
    {
        builder.HasData(
            new StudyFormat
            {
                Id = 1,
                Name = "Presencial"
            },
            new StudyFormat
            {
                Id = 2,
                Name = "EAD"
            },
            new StudyFormat
            {
                Id = 3,
                Name = "Híbrido"
            }
        );
    }
}
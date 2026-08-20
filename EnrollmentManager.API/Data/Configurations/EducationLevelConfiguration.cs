using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnrollmentManager.API.Models;

namespace EnrollmentManager.API.Data.Configurations
{
    public class EducationlevelConfiguration : IEntityTypeConfiguration<EducationLevel>
    {
        public void Configure(EntityTypeBuilder<EducationLevel> builder){
            // Aqui você pode colocar regras da tabela se quiser (ex: MaxLength, Required)
            builder.Property(r => r.Name).IsRequired().HasMaxLength(50);

            // O Seed fica aqui dentro, organizadinho!
            builder.HasData(
                new EducationLevel { Id = 1, Name = "Graduação" },
                new EducationLevel { Id = 2, Name = "Pós-graduação" },
                new EducationLevel { Id = 3, Name = "Mestrado" },
                new EducationLevel { Id = 4, Name = "Doutorado" }
            );
        }
    }
}
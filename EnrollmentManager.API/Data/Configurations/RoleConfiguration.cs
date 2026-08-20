using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnrollmentManager.API.Models;

namespace EnrollmentManager.API.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
      public void Configure(EntityTypeBuilder<Role> builder){
            // Aqui você pode colocar regras da tabela se quiser (ex: MaxLength, Required)
            builder.Property(r => r.Name).IsRequired().HasMaxLength(50);

            // O Seed fica aqui dentro, organizadinho!
            builder.HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Student" }
            );
        }
    }
}
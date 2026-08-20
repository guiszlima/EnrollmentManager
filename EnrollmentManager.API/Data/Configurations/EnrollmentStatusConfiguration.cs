using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnrollmentManager.API.Models;

namespace EnrollmentManager.API.Data.Configurations;

public class EnrollmentStatusConfiguration : IEntityTypeConfiguration<EnrollmentStatus>
{
    public void Configure(EntityTypeBuilder<EnrollmentStatus> builder)
    {
        // Regras da tabela baseadas nos DataAnnotations que você já colocou na Model
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(s => s.Description)
            .HasMaxLength(200);

        // Índice único no Code (caso o EF não pegue direto do atributo [Index])
        builder.HasIndex(s => s.Code)
            .IsUnique();

        // Seed Data com os status essenciais da matrícula
        builder.HasData(
            new EnrollmentStatus 
            { 
                Id = 1, 
                Name = "Pendente", 
                Code = "PENDING", 
                Description = "Matrícula realizada, aguardando validação de documentos ou pagamento." 
            },
            new EnrollmentStatus 
            { 
                Id = 2, 
                Name = "Aprovada", 
                Code = "APPROVED", 
                Description = "Matrícula ativa e aprovada pela instituição." 
            },
            new EnrollmentStatus 
            { 
                Id = 3, 
                Name = "Trancada", 
                Code = "SUSPENDED", 
                Description = "Matrícula temporariamente pausada pelo aluno." 
            },
            new EnrollmentStatus 
            { 
                Id = 4, 
                Name = "Cancelada", 
                Code = "CANCELLED", 
                Description = "Matrícula cancelada antes ou durante o período letivo." 
            },
            new EnrollmentStatus 
            { 
                Id = 5, 
                Name = "Concluída", 
                Code = "COMPLETED", 
                Description = "Aluno finalizou com sucesso o programa." 
            }
        );
    }
}
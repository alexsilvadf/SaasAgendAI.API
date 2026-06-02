using AgendAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendAI.Infra.Persistence.Configurations;

public class ChamadaPainelTvAtualConfiguration : IEntityTypeConfiguration<ChamadaPainelTvAtual>
{
    public void Configure(EntityTypeBuilder<ChamadaPainelTvAtual> builder)
    {
        builder.ToTable("PainelTvChamadaAtual");

        builder.HasKey(chamada => chamada.Id);

        builder.Property(chamada => chamada.Id)
            .ValueGeneratedOnAdd();

        builder.Property(chamada => chamada.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(chamada => chamada.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(chamada => chamada.TenantId)
            .IsUnique()
            .HasDatabaseName("IX_PainelTvChamadaAtual_TenantId");

        builder.Property(chamada => chamada.PacienteNome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(chamada => chamada.ProfissionalNome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(chamada => chamada.ProfissionalId);
        builder.Property(chamada => chamada.AgendamentoId);
        builder.Property(chamada => chamada.PacienteId);

        builder.Property(chamada => chamada.Horario)
            .HasMaxLength(10);

        builder.Property(chamada => chamada.Timestamp)
            .IsRequired();
    }
}

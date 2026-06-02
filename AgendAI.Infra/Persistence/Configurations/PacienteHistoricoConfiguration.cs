using AgendAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendAI.Infra.Persistence.Configurations;

public class PacienteHistoricoConfiguration : IEntityTypeConfiguration<PacienteHistorico>
{
    public void Configure(EntityTypeBuilder<PacienteHistorico> builder)
    {
        builder.ToTable("PacienteHistoricos");

        builder.HasKey(historico => historico.Id);

        builder.Property(historico => historico.Procedimento).HasMaxLength(200).IsRequired();
        builder.Property(historico => historico.Profissional).HasMaxLength(200).IsRequired();
        builder.Property(historico => historico.Observacoes).HasMaxLength(2000);

        builder.Property(historico => historico.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(historico => historico.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(historico => historico.Paciente)
            .WithMany(paciente => paciente.Historicos)
            .HasForeignKey(historico => historico.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


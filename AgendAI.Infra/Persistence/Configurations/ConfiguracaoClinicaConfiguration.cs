using AgendAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendAI.Infra.Persistence.Configurations;

public class ConfiguracaoClinicaConfiguration : IEntityTypeConfiguration<ConfiguracaoClinica>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoClinica> builder)
    {
        builder.ToTable("ConfiguracoesClinica");

        builder.HasKey(configuracao => configuracao.Id);

        builder.Property(configuracao => configuracao.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(configuracao => configuracao.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(configuracao => configuracao.TenantId)
            .IsUnique()
            .HasDatabaseName("IX_ConfiguracoesClinica_TenantId");
    }
}

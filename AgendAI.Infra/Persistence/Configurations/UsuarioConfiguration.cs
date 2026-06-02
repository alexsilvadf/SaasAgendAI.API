using AgendAI.Domain.Entities;
using AgendAI.Domain.Enums;
using AgendAI.Infra.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendAI.Infra.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(usuario => usuario.Id);

        builder.Property(usuario => usuario.Nome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(usuario => usuario.Login)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(usuario => usuario.Email)
            .HasMaxLength(256);

        builder.Property(usuario => usuario.SenhaHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(usuario => usuario.Role)
            .HasConversion(EnumSnakeCaseConverter.Create<UserRole>())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(usuario => usuario.Especialidade)
            .HasMaxLength(200);

        builder.Property(usuario => usuario.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(usuario => usuario.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(usuario => new { usuario.TenantId, usuario.Login })
            .IsUnique()
            .HasDatabaseName("IX_Usuarios_Login");

        builder.HasIndex(usuario => new { usuario.TenantId, usuario.Email })
            .IsUnique()
            .HasDatabaseName("IX_Usuarios_Email")
            .HasFilter("\"Email\" IS NOT NULL AND \"Email\" <> ''");
    }
}

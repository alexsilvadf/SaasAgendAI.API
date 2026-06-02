using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Tenants;
using AgendAI.Domain.Entities;
using AgendAI.Domain.Enums;
using AgendAI.Domain.Exceptions;
using AgendAI.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infra.Services;

public sealed class TenantProvisioningService(AgendAiDbContext db) : ITenantProvisioningService
{
    public async Task<RegisterTenantResponse> RegisterAsync(
        RegisterTenantRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        var slug = request.Slug.Trim().ToLowerInvariant();
        var login = request.AdminLogin.Trim().ToLowerInvariant();
        var email = request.AdminEmail.Trim().ToLowerInvariant();

        var slugExists = await db.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.Slug == slug, cancellationToken);
        if (slugExists)
            throw new ConflictException("Slug já está em uso.");

        var now = DateTime.UtcNow;
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Nome = request.NomeClinica.Trim(),
            Slug = slug,
            Ativo = true,
            CriadoEm = now
        };

        var admin = new Usuario
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Nome = request.AdminNome.Trim(),
            Login = login,
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.AdminSenha),
            Role = UserRole.Administrador,
            Ativo = true,
            CriadoEm = now
        };

        var configuracao = new ConfiguracaoClinica
        {
            TenantId = tenant.Id,
            HoraAbertura = new TimeOnly(8, 0),
            HoraFechamento = new TimeOnly(18, 0),
            IntervaloMinutos = 30
        };

        var painel = new ChamadaPainelTvAtual
        {
            TenantId = tenant.Id,
            PacienteNome = string.Empty,
            ProfissionalNome = string.Empty,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.Tenants.Add(tenant);
        db.Usuarios.Add(admin);
        db.ConfiguracoesClinica.Add(configuracao);
        db.ChamadasPainelTvAtual.Add(painel);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new RegisterTenantResponse
        {
            TenantId = tenant.Id,
            TenantSlug = tenant.Slug,
            TenantNome = tenant.Nome,
            AdminUserId = admin.Id
        };
    }

    private static void Validate(RegisterTenantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NomeClinica)
            || string.IsNullOrWhiteSpace(request.Slug)
            || string.IsNullOrWhiteSpace(request.AdminNome)
            || string.IsNullOrWhiteSpace(request.AdminLogin)
            || string.IsNullOrWhiteSpace(request.AdminEmail)
            || string.IsNullOrWhiteSpace(request.AdminSenha))
        {
            throw new ConflictException("Todos os campos de cadastro da clínica são obrigatórios.");
        }
    }
}


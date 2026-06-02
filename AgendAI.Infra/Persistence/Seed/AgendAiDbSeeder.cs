using AgendAI.Domain.Entities;
using AgendAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infra.Persistence.Seed;

public static class AgendAiDbSeeder
{
    private static readonly Dictionary<string, string> EmailsPorLogin = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"] = "admin@agendai.local",
        ["ana.martins"] = "ana.martins@agendai.local",
        ["bruno.costa"] = "bruno.costa@agendai.local",
        ["carla"] = "carla@agendai.local",
        ["carla.dias"] = "carla.dias@agendai.local",
        ["raissa"] = "raissa@agendai.local"
    };

    /// <summary>
    /// Cria apenas usuários iniciais para login em banco vazio.
    /// Demais dados (pacientes, agendamentos, etc.) devem ser cadastrados pela aplicação.
    /// </summary>
    public static async Task SeedAsync(AgendAiDbContext db, CancellationToken cancellationToken = default)
    {
        await GarantirTenantDefaultAsync(db, cancellationToken);

        if (!await db.Usuarios.IgnoreQueryFilters().AnyAsync(u => u.TenantId == SeedIds.TenantDefault, cancellationToken))
        {
            var now = DateTime.UtcNow;
            db.Usuarios.AddRange(CriarUsuarios(now));
            await db.SaveChangesAsync(cancellationToken);
        }

        await GarantirConfiguracaoClinicaDefaultAsync(db, cancellationToken);
        await GarantirPainelTvAtualDefaultAsync(db, cancellationToken);

        await GarantirTenantDemoAsync(
            db,
            SeedIds.TenantClinicaSorriso,
            "clinica-sorriso",
            "Clínica Sorriso",
            SeedIds.UsuarioAdminSorriso,
            "Admin Sorriso",
            "admin.sorriso@agendai.local",
            cancellationToken);
        await GarantirTenantDemoAsync(
            db,
            SeedIds.TenantClinicaVital,
            "clinica-vital",
            "Clínica Vital",
            SeedIds.UsuarioAdminVital,
            "Admin Vital",
            "admin.vital@agendai.local",
            cancellationToken);

        await AtualizarEmailsUsuariosSeedAsync(db, cancellationToken);
        await GarantirUsuarioRaissaAsync(db, cancellationToken);
    }

    private static List<Usuario> CriarUsuarios(DateTime now) =>
    [
        CriarUsuario(SeedIds.UsuarioAdmin, SeedIds.TenantDefault, "Admin Sistema", "admin", "admin@agendai.local", UserRole.Administrador, null, true, now, "admin123"),
        CriarUsuario(SeedIds.UsuarioAnaMartins, SeedIds.TenantDefault, "Dra. Ana Martins", "ana.martins", "ana.martins@agendai.local", UserRole.Dentista, "Clínica geral", true, now),
        CriarUsuario(SeedIds.UsuarioBrunoCosta, SeedIds.TenantDefault, "Dr. Bruno Costa", "bruno.costa", "bruno.costa@agendai.local", UserRole.Dentista, "Cardiologia", true, now),
        CriarUsuario(SeedIds.UsuarioCarlaRecepcao, SeedIds.TenantDefault, "Carla Recepção", "carla", "carla@agendai.local", UserRole.Recepcionista, null, true, now),
        CriarUsuario(SeedIds.UsuarioCarlaDias, SeedIds.TenantDefault, "Dra. Carla Dias", "carla.dias", "carla.dias@agendai.local", UserRole.Dentista, "Dermatologia", false, now),
        CriarUsuario(SeedIds.UsuarioRaissa, SeedIds.TenantDefault, "Dra. Raissa", "raissa", "raissa@agendai.local", UserRole.Dentista, "Dentista", true, now)
    ];

    private static Usuario CriarUsuario(
        Guid id,
        Guid tenantId,
        string nome,
        string login,
        string email,
        UserRole role,
        string? especialidade,
        bool ativo,
        DateTime now,
        string senha = "senha123") =>
        new()
        {
            Id = id,
            TenantId = tenantId,
            Nome = nome,
            Login = login,
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
            Role = role,
            Especialidade = especialidade,
            Ativo = ativo,
            CriadoEm = now
        };

    private static async Task AtualizarEmailsUsuariosSeedAsync(
        AgendAiDbContext db,
        CancellationToken cancellationToken)
    {
        var usuarios = await db.Usuarios.IgnoreQueryFilters().ToListAsync(cancellationToken);
        var alterou = false;

        foreach (var usuario in usuarios)
        {
            if (!string.IsNullOrWhiteSpace(usuario.Email))
                continue;

            if (!EmailsPorLogin.TryGetValue(usuario.Login, out var email))
                continue;

            usuario.Email = email;
            alterou = true;
        }

        if (alterou)
            await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task GarantirUsuarioRaissaAsync(
        AgendAiDbContext db,
        CancellationToken cancellationToken)
    {
        var jaExiste = await db.Usuarios.IgnoreQueryFilters().AnyAsync(
            u => u.TenantId == SeedIds.TenantDefault &&
                 (u.Id == SeedIds.UsuarioRaissa || u.Login == "raissa"),
            cancellationToken);

        if (jaExiste)
            return;

        var now = DateTime.UtcNow;
        db.Usuarios.Add(CriarUsuario(
            SeedIds.UsuarioRaissa,
            SeedIds.TenantDefault,
            "Dra. Raissa",
            "raissa",
            "raissa@agendai.local",
            UserRole.Dentista,
            "Dentista",
            true,
            now));

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task GarantirTenantDefaultAsync(
        AgendAiDbContext db,
        CancellationToken cancellationToken)
    {
        await GarantirTenantAsync(
            db,
            SeedIds.TenantDefault,
            "default",
            "Clínica Padrão",
            cancellationToken);
    }

    private static async Task GarantirTenantDemoAsync(
        AgendAiDbContext db,
        Guid tenantId,
        string slug,
        string nome,
        Guid adminUserId,
        string adminNome,
        string adminEmail,
        CancellationToken cancellationToken)
    {
        await GarantirTenantAsync(db, tenantId, slug, nome, cancellationToken);
        await GarantirConfiguracaoClinicaAsync(db, tenantId, cancellationToken);
        await GarantirPainelTvAtualAsync(db, tenantId, cancellationToken);

        var adminExiste = await db.Usuarios.IgnoreQueryFilters().AnyAsync(
            u => u.TenantId == tenantId && u.Login == "admin",
            cancellationToken);
        if (adminExiste)
            return;

        var now = DateTime.UtcNow;
        db.Usuarios.Add(CriarUsuario(
            adminUserId,
            tenantId,
            adminNome,
            "admin",
            adminEmail,
            UserRole.Administrador,
            null,
            true,
            now,
            "admin123"));

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task GarantirTenantAsync(
        AgendAiDbContext db,
        Guid tenantId,
        string slug,
        string nome,
        CancellationToken cancellationToken)
    {
        var tenantExiste = await db.Tenants.AnyAsync(t => t.Id == tenantId, cancellationToken);
        if (tenantExiste)
            return;

        db.Tenants.Add(new Tenant
        {
            Id = tenantId,
            Nome = nome,
            Slug = slug,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task GarantirConfiguracaoClinicaDefaultAsync(
        AgendAiDbContext db,
        CancellationToken cancellationToken) =>
        await GarantirConfiguracaoClinicaAsync(db, SeedIds.TenantDefault, cancellationToken);

    private static async Task GarantirConfiguracaoClinicaAsync(
        AgendAiDbContext db,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var existe = await db.ConfiguracoesClinica
            .AnyAsync(c => c.TenantId == tenantId, cancellationToken);

        if (existe)
            return;

        var configId = await IntEntityIdAllocator.NextConfiguracaoClinicaIdAsync(db, cancellationToken);
        db.ConfiguracoesClinica.Add(new ConfiguracaoClinica
        {
            Id = configId,
            TenantId = tenantId,
            HoraAbertura = new TimeOnly(8, 0),
            HoraFechamento = new TimeOnly(18, 0),
            IntervaloMinutos = 30
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task GarantirPainelTvAtualDefaultAsync(
        AgendAiDbContext db,
        CancellationToken cancellationToken) =>
        await GarantirPainelTvAtualAsync(db, SeedIds.TenantDefault, cancellationToken);

    private static async Task GarantirPainelTvAtualAsync(
        AgendAiDbContext db,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var existe = await db.ChamadasPainelTvAtual
            .AnyAsync(c => c.TenantId == tenantId, cancellationToken);

        if (existe)
            return;

        var painelId = await IntEntityIdAllocator.NextChamadaPainelTvIdAsync(db, cancellationToken);
        db.ChamadasPainelTvAtual.Add(new ChamadaPainelTvAtual
        {
            Id = painelId,
            TenantId = tenantId,
            PacienteNome = string.Empty,
            ProfissionalNome = string.Empty,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}

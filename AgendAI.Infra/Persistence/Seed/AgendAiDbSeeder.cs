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
        if (!await db.Usuarios.AnyAsync(cancellationToken))
        {
            var now = DateTime.UtcNow;
            db.Usuarios.AddRange(CriarUsuarios(now));
            await db.SaveChangesAsync(cancellationToken);
        }

        await AtualizarEmailsUsuariosSeedAsync(db, cancellationToken);
        await GarantirUsuarioRaissaAsync(db, cancellationToken);
    }

    private static List<Usuario> CriarUsuarios(DateTime now) =>
    [
        CriarUsuario(SeedIds.UsuarioAdmin, "Admin Sistema", "admin", "admin@agendai.local", UserRole.Administrador, null, true, now, "admin123"),
        CriarUsuario(SeedIds.UsuarioAnaMartins, "Dra. Ana Martins", "ana.martins", "ana.martins@agendai.local", UserRole.Dentista, "Clínica geral", true, now),
        CriarUsuario(SeedIds.UsuarioBrunoCosta, "Dr. Bruno Costa", "bruno.costa", "bruno.costa@agendai.local", UserRole.Dentista, "Cardiologia", true, now),
        CriarUsuario(SeedIds.UsuarioCarlaRecepcao, "Carla Recepção", "carla", "carla@agendai.local", UserRole.Recepcionista, null, true, now),
        CriarUsuario(SeedIds.UsuarioCarlaDias, "Dra. Carla Dias", "carla.dias", "carla.dias@agendai.local", UserRole.Dentista, "Dermatologia", false, now),
        CriarUsuario(SeedIds.UsuarioRaissa, "Dra. Raissa", "raissa", "raissa@agendai.local", UserRole.Dentista, "Dentista", true, now)
    ];

    private static Usuario CriarUsuario(
        Guid id,
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
        var usuarios = await db.Usuarios.ToListAsync(cancellationToken);
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
        var jaExiste = await db.Usuarios.AnyAsync(
            u => u.Id == SeedIds.UsuarioRaissa || u.Login == "raissa",
            cancellationToken);

        if (jaExiste)
            return;

        var now = DateTime.UtcNow;
        db.Usuarios.Add(CriarUsuario(
            SeedIds.UsuarioRaissa,
            "Dra. Raissa",
            "raissa",
            "raissa@agendai.local",
            UserRole.Dentista,
            "Dentista",
            true,
            now));

        await db.SaveChangesAsync(cancellationToken);
    }
}

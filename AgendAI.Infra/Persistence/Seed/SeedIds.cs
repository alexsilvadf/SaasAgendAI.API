namespace AgendAI.Infra.Persistence.Seed;

/// <summary>IDs fixos dos usuários criados no bootstrap (banco vazio).</summary>
public static class SeedIds
{
    public static readonly Guid TenantDefault = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid TenantClinicaSorriso = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public static readonly Guid TenantClinicaVital = Guid.Parse("00000000-0000-0000-0000-000000000003");

    public static readonly Guid UsuarioAdmin = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    public static readonly Guid UsuarioAnaMartins = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
    public static readonly Guid UsuarioBrunoCosta = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
    public static readonly Guid UsuarioCarlaRecepcao = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4");
    public static readonly Guid UsuarioCarlaDias = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5");
    public static readonly Guid UsuarioRaissa = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6");
    public static readonly Guid UsuarioAdminSorriso = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
    public static readonly Guid UsuarioAdminVital = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
}

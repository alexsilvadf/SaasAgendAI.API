namespace AgendAI.Application.DTOs.Tenants;

public sealed class RegisterTenantRequest
{
    public string NomeClinica { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string AdminNome { get; set; } = string.Empty;

    public string AdminLogin { get; set; } = string.Empty;

    public string AdminEmail { get; set; } = string.Empty;

    public string AdminSenha { get; set; } = string.Empty;
}


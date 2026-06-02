using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Auth;
using AgendAI.Application.Options;
using AgendAI.Application.Security;
using AgendAI.Domain.Entities;
using AgendAI.Domain.Enums;
using AgendAI.Domain.Exceptions;
using AgendAI.Infra.Persistence;
using AgendAI.Infra.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AgendAI.Infra.Services;

public sealed class AuthService(
    AgendAiDbContext db,
    JwtTokenGenerator tokenGenerator,
    IEmailSender emailSender,
    IOptions<AppOptions> appOptions) : IAuthService
{
    private const string MensagemRecuperacaoGenerica =
        "Se os dados estiverem corretos, você receberá um e-mail com instruções para redefinir sua senha.";

    private const int TokenValidadeHoras = 1;
    private const int SenhaMinima = 8;

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.TenantSlug)
            || string.IsNullOrWhiteSpace(request.Usuario)
            || string.IsNullOrWhiteSpace(request.Senha))
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        var tenantSlug = request.TenantSlug.Trim().ToLowerInvariant();
        var login = request.Usuario.Trim().ToLowerInvariant();
        var tenant = await db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == tenantSlug, cancellationToken);

        if (tenant is null || !tenant.Ativo)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        var usuario = await db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.TenantId == tenant.Id && u.Login == login, cancellationToken);

        if (usuario is null || !usuario.Ativo)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        var senhaValida = false;
        try
        {
            senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash);
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");
        }

        if (!senhaValida)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        var (token, expiresIn) = tokenGenerator.Generate(usuario, tenant);

        return new LoginResponse
        {
            Token = token,
            ExpiresIn = expiresIn,
            Nome = usuario.Nome,
            Usuario = usuario.Login,
            Role = usuario.Role.ToJsonValue(),
            Permissions = RolePermissions.GetPermissionNames(usuario.Role),
            ProfessionalId = usuario.Role == UserRole.Dentista ? usuario.Id : null,
            TenantId = tenant.Id,
            TenantSlug = tenant.Slug,
            TenantNome = tenant.Nome
        };
    }

    public async Task<MessageResponse> SolicitarRecuperacaoSenhaAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.TenantSlug) || string.IsNullOrWhiteSpace(request.Identificador))
            return new MessageResponse { Message = MensagemRecuperacaoGenerica };

        var tenantSlug = request.TenantSlug.Trim().ToLowerInvariant();
        var identificador = request.Identificador.Trim().ToLowerInvariant();
        var tenant = await db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == tenantSlug && t.Ativo, cancellationToken);

        if (tenant is null)
            return new MessageResponse { Message = MensagemRecuperacaoGenerica };

        var usuario = await BuscarUsuarioPorIdentificadorAsync(tenant.Id, identificador, cancellationToken);

        if (usuario is null || !usuario.Ativo || string.IsNullOrWhiteSpace(usuario.Email))
            return new MessageResponse { Message = MensagemRecuperacaoGenerica };

        var rawToken = PasswordRecoveryTokenHelper.GerarToken();
        var tokenHash = PasswordRecoveryTokenHelper.HashToken(rawToken);
        var expiraEm = DateTime.UtcNow.AddHours(TokenValidadeHoras);

        var tokensAntigos = await db.TokensRecuperacaoSenha
            .Where(t => t.UsuarioId == usuario.Id && !t.Utilizado)
            .ToListAsync(cancellationToken);

        foreach (var antigo in tokensAntigos)
            antigo.Utilizado = true;

        db.TokensRecuperacaoSenha.Add(new TokenRecuperacaoSenha
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            UsuarioId = usuario.Id,
            TokenHash = tokenHash,
            ExpiraEm = expiraEm,
            Utilizado = false
        });

        await db.SaveChangesAsync(cancellationToken);

        var baseUrl = appOptions.Value.FrontendBaseUrl.TrimEnd('/');
        var link = $"{baseUrl}/redefinir-senha?token={Uri.EscapeDataString(rawToken)}";
        var assunto = "AgendAI — Redefinição de senha";
        var corpo = $"""
            <p>Olá, <strong>{usuario.Nome}</strong>.</p>
            <p>Recebemos uma solicitação para redefinir sua senha no AgendAI.</p>
            <p><a href="{link}">Clique aqui para criar uma nova senha</a></p>
            <p>O link expira em {TokenValidadeHoras} hora(s). Se você não solicitou, ignore este e-mail.</p>
            <p style="color:#64748b;font-size:12px">Ou copie o link: {link}</p>
            """;

        try
        {
            await emailSender.SendAsync(usuario.Email!, assunto, corpo, cancellationToken);
        }
        catch
        {
            // Não revela falha de envio ao cliente.
        }

        return new MessageResponse { Message = MensagemRecuperacaoGenerica };
    }

    public async Task<MessageResponse> RedefinirSenhaAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            throw new ConflictException("Link inválido ou expirado.");

        if (string.IsNullOrWhiteSpace(request.NovaSenha) || request.NovaSenha.Length < SenhaMinima)
            throw new ConflictException($"A nova senha deve ter pelo menos {SenhaMinima} caracteres.");

        if (request.NovaSenha != request.ConfirmarSenha)
            throw new ConflictException("A confirmação da senha não confere.");

        var tokenHash = PasswordRecoveryTokenHelper.HashToken(request.Token.Trim());
        var agora = DateTime.UtcNow;

        var registro = await db.TokensRecuperacaoSenha
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(
                t => t.TokenHash == tokenHash && !t.Utilizado && t.ExpiraEm > agora,
                cancellationToken);

        if (registro is null || !registro.Usuario.Ativo)
            throw new ConflictException("Link inválido ou expirado.");

        registro.Utilizado = true;
        registro.Usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);

        var outrosTokens = await db.TokensRecuperacaoSenha
            .Where(t => t.UsuarioId == registro.UsuarioId && !t.Utilizado && t.Id != registro.Id)
            .ToListAsync(cancellationToken);

        foreach (var outro in outrosTokens)
            outro.Utilizado = true;

        await db.SaveChangesAsync(cancellationToken);

        return new MessageResponse { Message = "Senha redefinida com sucesso. Faça login com a nova senha." };
    }

    private async Task<Usuario?> BuscarUsuarioPorIdentificadorAsync(
        Guid tenantId,
        string identificador,
        CancellationToken cancellationToken)
    {
        if (identificador.Contains('@'))
        {
            return await db.Usuarios
                .FirstOrDefaultAsync(
                    u => u.TenantId == tenantId && u.Email != null && u.Email.ToLower() == identificador,
                    cancellationToken);
        }

        return await db.Usuarios
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Login == identificador, cancellationToken);
    }
}

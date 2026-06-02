using System.IdentityModel.Tokens.Jwt;
using AgendAI.Application.Abstractions;
using AgendAI.Application.Options;
using AgendAI.Application.DTOs.Auth;
using AgendAI.Application.DTOs.Pacientes;
using AgendAI.Domain.Enums;
using AgendAI.Infra.Persistence;
using AgendAI.Infra.Persistence.Seed;
using AgendAI.Infra.Security;
using AgendAI.Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AgendAI.Integration.Tests;

public class BaselineIntegrationTests
{
    private sealed class FixedTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;

        public bool IsResolved => TenantId != Guid.Empty;
    }

    private sealed class NoopEmailSender : IEmailSender
    {
        public Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private static AgendAiDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AgendAiDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AgendAiDbContext(options);
    }

    private static AuthService CreateAuthService(AgendAiDbContext db)
    {
        var jwtSettings = Options.Create(new JwtSettings
        {
            Secret = "unit-test-super-secret-unit-test-super-secret",
            Issuer = "AgendAI",
            Audience = "AgendAI.App",
            ExpirationMinutes = 60
        });

        var appOptions = Options.Create(new AppOptions
        {
            FrontendBaseUrl = "http://localhost:4200"
        });

        var tokenGenerator = new JwtTokenGenerator(jwtSettings);
        var emailSender = new NoopEmailSender();

        return new AuthService(db, tokenGenerator, emailSender, appOptions);
    }

    [Fact]
    public async Task Login_baseline_seedShouldReturnToken()
    {
        var dbName = $"baseline_seed_{Guid.NewGuid():N}";
        await using var db = CreateDbContext(dbName);
        await AgendAiDbSeeder.SeedAsync(db);

        var auth = CreateAuthService(db);

        var response = await auth.LoginAsync(new LoginRequest
        {
            TenantSlug = "default",
            Usuario = "admin",
            Senha = "admin123"
        });

        Assert.False(string.IsNullOrWhiteSpace(response.Token));

        // Sanity-check that token is structurally a JWT.
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(response.Token);
        Assert.False(string.IsNullOrWhiteSpace(jwt.Subject));
    }

    [Fact]
    public async Task Paciente_criar_e_obter_shouldWork()
    {
        var dbName = $"paciente_criar_{Guid.NewGuid():N}";
        await using var db = CreateDbContext(dbName);

        // Seed users so Auth-related constraints / FK graphs stay consistent in the future.
        await AgendAiDbSeeder.SeedAsync(db);

        var pacienteService = new PacienteService(db);

        var created = await pacienteService.CriarAsync(new SalvarPacienteRequest
        {
            Nome = "Paciente Teste",
            Cpf = "123.456.789-09",
            DataNascimento = "1990-01-01",
            Sexo = "masculino",
            EstadoCivil = "solteiro",
            Telefone = "11999999999",
            Email = "paciente.teste@agendai.local",
            Cep = "01000-000",
            Logradouro = "Rua A",
            Numero = "123",
            Complemento = "Apto 1",
            Bairro = "Centro",
            Cidade = "Sao Paulo",
            Uf = "SP",
            TipoSanguineo = "A+",
            Ativo = true,
            Anamnese = new AnamneseDto
            {
                TemDoencaCardiaca = false,
                TemDiabetes = false,
                TemHipertensao = false,
                TemCoagulopatia = false,
                TemAlergiaMedicamento = false,
                AlergiaMedicamentoDesc = "—",
                TemAlergiaMaterial = false,
                AlergiaMaterialDesc = "—",
                UsaMedicamentoContinuo = false,
                MedicamentoContinuoDesc = "—",
                EstaGravida = false,
                Fumante = false,
                ObservacoesGerais = "sem observações"
            }
        });

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("123.456.789-09", created.Cpf);

        var fetched = await pacienteService.ObterPorIdAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(created.Cpf, fetched.Cpf);
    }

    [Fact]
    public async Task CrossTenant_pacienteCriadoNoTenantA_naoApareceNoTenantB()
    {
        var dbName = $"cross_tenant_{Guid.NewGuid():N}";
        var tenantA = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var tenantB = Guid.Parse("22222222-2222-2222-2222-222222222222");

        await using (var setupDb = CreateDbContext(dbName))
        {
            setupDb.Tenants.AddRange(
                new AgendAI.Domain.Entities.Tenant { Id = tenantA, Nome = "Tenant A", Slug = "tenant-a", Ativo = true, CriadoEm = DateTime.UtcNow },
                new AgendAI.Domain.Entities.Tenant { Id = tenantB, Nome = "Tenant B", Slug = "tenant-b", Ativo = true, CriadoEm = DateTime.UtcNow });
            await setupDb.SaveChangesAsync();
        }

        await using var dbA = new AgendAiDbContext(
            new DbContextOptionsBuilder<AgendAiDbContext>().UseInMemoryDatabase(dbName).Options,
            new FixedTenantContext(tenantA));
        await using var dbB = new AgendAiDbContext(
            new DbContextOptionsBuilder<AgendAiDbContext>().UseInMemoryDatabase(dbName).Options,
            new FixedTenantContext(tenantB));

        var now = DateTime.UtcNow;
        var pacienteA = new AgendAI.Domain.Entities.Paciente
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Nome = "Paciente Tenant A",
            Cpf = "98765432100",
            DataNascimento = new DateOnly(1992, 2, 2),
            Sexo = Sexo.Feminino,
            EstadoCivil = EstadoCivil.Solteiro,
            Telefone = "11911111111",
            Email = "tenant.a@agendai.local",
            Cep = "01000000",
            Logradouro = "Rua Tenant A",
            Numero = "1",
            Complemento = "Sala 1",
            Bairro = "Centro",
            Cidade = "Sao Paulo",
            Uf = "SP",
            TipoSanguineo = TipoSanguineo.OPositivo,
            Ativo = true,
            CriadoEm = now,
            AtualizadoEm = now,
            Anamnese = new AgendAI.Domain.Entities.PacienteAnamnese
            {
                TenantId = tenantA,
                ObservacoesGerais = string.Empty
            }
        };
        pacienteA.Anamnese.PacienteId = pacienteA.Id;
        dbA.Pacientes.Add(pacienteA);
        await dbA.SaveChangesAsync();

        var pacienteServiceB = new PacienteService(dbB);

        var listA = await dbA.Pacientes.AsNoTracking().ToListAsync();
        var listB = await pacienteServiceB.ListarAsync(null);

        Assert.Contains(listA, p => p.Id == pacienteA.Id);
        Assert.DoesNotContain(listB, p => p.Id == pacienteA.Id);

        var getByIdOnB = await pacienteServiceB.ObterPorIdAsync(pacienteA.Id);
        Assert.Null(getByIdOnB);
    }
}

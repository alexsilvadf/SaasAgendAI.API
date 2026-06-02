using AgendAI.Application.Abstractions;
using AgendAI.Domain.Entities;
using AgendAI.Infra.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infra.Persistence;

public class AgendAiDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AgendAiDbContext(
        DbContextOptions<AgendAiDbContext> options,
        ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    /// <summary>Usado em testes e bootstrap sem HTTP (sem filtro de tenant).</summary>
    public AgendAiDbContext(DbContextOptions<AgendAiDbContext> options)
        : this(options, new NullTenantContext())
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Paciente> Pacientes => Set<Paciente>();

    public DbSet<PacienteAnamnese> PacienteAnamneses => Set<PacienteAnamnese>();

    public DbSet<PacienteHistorico> PacienteHistoricos => Set<PacienteHistorico>();

    public DbSet<Procedimento> Procedimentos => Set<Procedimento>();

    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();

    public DbSet<BloqueioAgenda> BloqueiosAgenda => Set<BloqueioAgenda>();

    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();

    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();

    public DbSet<TokenRecuperacaoSenha> TokensRecuperacaoSenha => Set<TokenRecuperacaoSenha>();

    public DbSet<ConfiguracaoClinica> ConfiguracoesClinica => Set<ConfiguracaoClinica>();

    public DbSet<ChamadaPainelTvAtual> ChamadasPainelTvAtual => Set<ChamadaPainelTvAtual>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgendAiDbContext).Assembly);
        modelBuilder.ApplyValorColumnPrecision();
        modelBuilder.ValidateEnumStringStorage();
        modelBuilder.ApplyRestrictOnDelete();
        ApplyTenantQueryFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<Paciente>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<PacienteAnamnese>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<PacienteHistorico>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<Procedimento>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<Agendamento>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<BloqueioAgenda>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<Atendimento>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<Lancamento>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<TokenRecuperacaoSenha>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<ConfiguracaoClinica>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<ChamadaPainelTvAtual>()
            .HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
    }
}

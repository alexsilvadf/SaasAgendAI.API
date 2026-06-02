using AgendAI.Application.Abstractions;
using AgendAI.Application.Options;
using AgendAI.Infra.Email;
using AgendAI.Infra.Persistence;
using AgendAI.Infra.Persistence.Interceptors;
using AgendAI.Infra.Security;
using AgendAI.Infra.Services;
using AgendAI.Infra.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AgendAI.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<TenantSaveChangesInterceptor>();

        if (DataOptions.UseInMemory(configuration))
        {
            services.AddDbContext<AgendAiDbContext>((serviceProvider, options) =>
                options.UseInMemoryDatabase(DataOptions.InMemoryDatabaseName)
                    .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>()));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não configurada.");

            services.AddDbContext<AgendAiDbContext>((serviceProvider, options) =>
                options.UseNpgsql(connectionString)
                    .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>()));
        }

        JwtSettingsConfiguration.Register(services, configuration);

        services.Configure<AppOptions>(configuration.GetSection(AppOptions.SectionName));
        services.AddAgendAIEmail(configuration);

        services.AddSingleton<JwtTokenGenerator>();
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAgendaService, AgendaService>();
        services.AddScoped<IAgendamentoService, AgendamentoService>();
        services.AddScoped<IPacienteService, PacienteService>();
        services.AddScoped<IProcedimentoService, ProcedimentoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAtendimentoService, AtendimentoService>();
        services.AddScoped<IFinanceiroService, FinanceiroService>();
        services.AddScoped<IPainelTvService, PainelTvService>();
        services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddAuthorization();

        return services;
    }
}

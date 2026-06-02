using AgendAI.API.Extensions;
using AgendAI.API.Middleware;
using AgendAI.Infra;
using AgendAI.Infra.Email;
using AgendAI.Infra.Persistence;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureCloudHosting();
builder.AddAgendAIConfiguration();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAgendAICors(builder.Configuration);
builder.Services.AddAgendAIExceptionHandling();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AgendAI API v1", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            []
        }
    });
});

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services, app.Environment);

app.Services.LogEmailMode();

app.UseAgendAIExceptionHandling();

var enableSwagger = app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("Swagger:Enabled");

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudHosting();

app.UseCors(CorsExtensions.AngularPolicyName);

app.UseAuthentication();
app.UseMiddleware<TenantRequiredMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/api/v1/health"));

app.Run();

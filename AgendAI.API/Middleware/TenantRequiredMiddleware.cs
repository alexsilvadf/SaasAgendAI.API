using AgendAI.Application.Abstractions;

namespace AgendAI.API.Middleware;

public sealed class TenantRequiredMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (RequiresTenant(context) && !tenantContext.IsResolved)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Clínica (tenant) não identificada no token. Saia e entre novamente informando o slug da clínica."
            });
            return;
        }

        await next(context);
    }

    private static bool RequiresTenant(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return false;

        var path = context.Request.Path;
        if (!path.StartsWithSegments("/api/v1", StringComparison.OrdinalIgnoreCase))
            return false;

        if (path.StartsWithSegments("/api/v1/auth", StringComparison.OrdinalIgnoreCase))
            return false;

        if (path.StartsWithSegments("/api/v1/tenants/register", StringComparison.OrdinalIgnoreCase))
            return false;

        if (path.StartsWithSegments("/api/v1/painel-tv", StringComparison.OrdinalIgnoreCase)
            && HttpMethods.IsGet(context.Request.Method))
            return false;

        if (path.StartsWithSegments("/api/v1/health", StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }
}

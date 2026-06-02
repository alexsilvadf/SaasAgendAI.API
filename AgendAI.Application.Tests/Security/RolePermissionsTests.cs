using AgendAI.Application.Security;
using AgendAI.Domain.Enums;
using Xunit;

namespace AgendAI.Application.Tests.Security;

/// <summary>
/// Contrato espelhado de <c>ROLE_PERMISSIONS</c> em
/// AgendAI.APP/src/app/core/models/user.model.ts
/// </summary>
public sealed class RolePermissionsTests
{
    private static readonly IReadOnlyDictionary<UserRole, string[]> Expected =
        new Dictionary<UserRole, string[]>
        {
            [UserRole.Administrador] =
            [
                "agenda:view", "agenda:edit",
                "agendamento:create", "agendamento:cancel",
                "pacientes:view", "pacientes:edit",
                "procedimentos:view", "procedimentos:edit",
                "financeiro:view", "financeiro:edit",
                "usuarios:view", "usuarios:edit",
                "atendimento:create"
            ],
            [UserRole.Dentista] =
            [
                "agenda:view",
                "agendamento:create", "agendamento:cancel",
                "pacientes:view",
                "procedimentos:view",
                "atendimento:create"
            ],
            [UserRole.Recepcionista] =
            [
                "agenda:view", "agenda:edit",
                "agendamento:create", "agendamento:cancel",
                "pacientes:view", "pacientes:edit"
            ]
        };

    [Theory]
    [InlineData(UserRole.Administrador)]
    [InlineData(UserRole.Dentista)]
    [InlineData(UserRole.Recepcionista)]
    public void GetPermissionNames_deve_coincidir_com_frontend(UserRole role)
    {
        var actual = RolePermissions.GetPermissionNames(role);

        Assert.Equal(Expected[role].OrderBy(p => p), actual.OrderBy(p => p));
    }

    [Fact]
    public void Todos_os_valores_Permission_devem_estar_atribuidos_a_algum_papel()
    {
        var cobertas = Enum.GetValues<UserRole>()
            .SelectMany(RolePermissions.GetPermissions)
            .Distinct()
            .OrderBy(p => p)
            .ToArray();

        var todas = Enum.GetValues<Permission>().OrderBy(p => p).ToArray();

        Assert.Equal(todas, cobertas);
    }
}

using AgendAI.Domain.Enums;

namespace AgendAI.Application.Security;

/// <summary>
/// Mapa RBAC papel → permissões. Deve permanecer idêntico a
/// <c>ROLE_PERMISSIONS</c> em AgendAI.APP/src/app/core/models/user.model.ts.
/// </summary>
public static class RolePermissions
{
    private static readonly IReadOnlyDictionary<UserRole, Permission[]> Map =
        new Dictionary<UserRole, Permission[]>
        {
            [UserRole.Administrador] =
            [
                Permission.AgendaView, Permission.AgendaEdit,
                Permission.AgendamentoCreate, Permission.AgendamentoCancel,
                Permission.PacientesView, Permission.PacientesEdit,
                Permission.ProcedimentosView, Permission.ProcedimentosEdit,
                Permission.FinanceiroView, Permission.FinanceiroEdit,
                Permission.UsuariosView, Permission.UsuariosEdit,
                Permission.AtendimentoCreate
            ],
            [UserRole.Dentista] =
            [
                Permission.AgendaView,
                Permission.AgendamentoCreate, Permission.AgendamentoCancel,
                Permission.PacientesView,
                Permission.ProcedimentosView,
                Permission.AtendimentoCreate
            ],
            [UserRole.Recepcionista] =
            [
                Permission.AgendaView, Permission.AgendaEdit,
                Permission.AgendamentoCreate, Permission.AgendamentoCancel,
                Permission.PacientesView, Permission.PacientesEdit
            ]
        };

    public static IReadOnlyList<Permission> GetPermissions(UserRole role) =>
        Map.TryGetValue(role, out var permissions) ? permissions : [];

    public static IReadOnlyList<string> GetPermissionNames(UserRole role) =>
        GetPermissions(role)
            .Select(p => p.ToJsonValue())
            .ToList();
}

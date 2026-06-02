# Permissões RBAC (espelho do AgendAI.APP)

Fonte de verdade no frontend: `AgendAI.APP/src/app/core/models/user.model.ts` (`ROLE_PERMISSIONS`).

Na API, o mapa equivalente está em `RolePermissions.cs`. Permissões **não são gravadas por usuário** no banco — derivam do `UserRole` no login (JWT + corpo da resposta).

## Papéis e permissões

| Permissão | Administrador | Dentista | Recepcionista |
|-----------|:-------------:|:--------:|:-------------:|
| `agenda:view` | ✓ | | ✓ |
| `agenda:edit` | ✓ | | ✓ |
| `agendamento:create` | ✓ | ✓ | ✓ |
| `agendamento:cancel` | ✓ | ✓ | ✓ |
| `pacientes:view` | ✓ | ✓ | ✓ |
| `pacientes:edit` | ✓ | | ✓ |
| `procedimentos:view` | ✓ | ✓ | |
| `procedimentos:edit` | ✓ | | |
| `financeiro:view` | ✓ | | |
| `financeiro:edit` | ✓ | | |
| `usuarios:view` | ✓ | | |
| `usuarios:edit` | ✓ | | |
| `atendimento:create` | ✓ | ✓ | |

## Usuários iniciais (bootstrap — apenas login)

| Login | Papel |
|-------|-------|
| `admin` / `admin123` | Administrador |
| `ana.martins` / `senha123` | Dentista |
| `bruno.costa` / `senha123` | Dentista |
| `carla` / `senha123` | Recepcionista |
| `carla.dias` / `senha123` | Dentista (inativo) |

Pacientes, procedimentos, agendamentos e demais registros **não são inseridos pelo seed**.

## Aplicação na API

1. **Login** — `AuthService` retorna `permissions` via `RolePermissions.GetPermissionNames`.
2. **JWT** — claims `permission` com os mesmos valores (`JwtTokenGenerator`).
3. **Controllers** — `User.HasPermission("...")` por endpoint.

Testes de contrato: `AgendAI.Application.Tests/Security/RolePermissionsTests.cs`.

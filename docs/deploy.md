# Deploy checklist (Render) — AgendAI.API

Este checklist garante que o serviço rode com **PostgreSQL persistente** e **segredos fora do repositório**.

## Variáveis obrigatórias

1. `ConnectionStrings__DefaultConnection`
   - Valor: string de conexão PostgreSQL do Render (não commitada).

2. `Jwt__Secret`
   - Deve existir e ter **mínimo 32 caracteres**.
   - No Render você pode usar `generateValue: true` (mantém segredo fora do repo).

3. `Jwt__Issuer`
   - Ex.: `AgendAI`

4. `Jwt__Audience`
   - Ex.: `AgendAI.App`

5. CORS
   - `Cors__AllowedOrigins__0` (origem web prod)
   - `Cors__AllowedOrigins__1` (origem local, se necessário)

## Variáveis que não devem ficar `true` em Production

- `Data__UseInMemory`
  - Deve ser `"false"` em Production (para usar PostgreSQL).

## Verificação pós-deploy

- Health endpoint:
  - `GET /api/v1/health`
- Reinício do serviço:
  - confirmar que dados não se perdem (deve persistir no PostgreSQL)


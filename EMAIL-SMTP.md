# Configuração SMTP — AgendAI API

A API envia e-mails reais quando `Email:IsConfigured` é verdadeiro (`Enabled` + host + usuário + senha + remetente).

Caso contrário, os links de recuperação de senha são gravados apenas no **log** da API.

## Desenvolvimento local

### Opção 1 — arquivo local (recomendado)

1. Copie o exemplo:
   ```bash
   cd AgendAI.API
   copy appsettings.Development.local.example.json appsettings.Development.local.json
   ```
2. Edite `appsettings.Development.local.json` com seus dados SMTP (arquivo já está no `.gitignore`).
3. Reinicie a API (`dotnet run`).

### Opção 2 — User Secrets

```bash
cd AgendAI.API
dotnet user-secrets set "Email:Enabled" "true"
dotnet user-secrets set "Email:FromAddress" "seu-email@gmail.com"
dotnet user-secrets set "Email:FromName" "AgendAI"
dotnet user-secrets set "Email:SmtpHost" "smtp.gmail.com"
dotnet user-secrets set "Email:SmtpPort" "587"
dotnet user-secrets set "Email:SmtpUser" "seu-email@gmail.com"
dotnet user-secrets set "Email:SmtpPassword" "xxxx xxxx xxxx xxxx"
dotnet user-secrets set "Email:Security" "StartTls"
```

## Gmail

1. Ative verificação em duas etapas na conta Google.
2. Crie uma **Senha de app** em https://myaccount.google.com/apppasswords
3. Use:
   - `SmtpHost`: `smtp.gmail.com`
   - `SmtpPort`: `587`
   - `Security`: `StartTls`
   - `SmtpUser` / `FromAddress`: seu e-mail Gmail completo
   - `SmtpPassword`: senha de app (16 caracteres, sem espaços)

## Outlook / Microsoft 365

- `SmtpHost`: `smtp.office365.com`
- `SmtpPort`: `587`
- `Security`: `StartTls`

## Produção (Render)

No painel do serviço, adicione variáveis de ambiente:

| Variável | Exemplo |
|----------|---------|
| `Email__Enabled` | `true` |
| `Email__SmtpHost` | `smtp.gmail.com` |
| `Email__SmtpPort` | `587` |
| `Email__SmtpUser` | `seu-email@gmail.com` |
| `Email__SmtpPassword` | senha de app |
| `Email__FromAddress` | `seu-email@gmail.com` |
| `Email__FromName` | `AgendAI` |
| `Email__Security` | `StartTls` |
| `App__FrontendBaseUrl` | `https://agendai-fawn.vercel.app` |

Reimplante o serviço após salvar.

## Verificar

Ao subir a API, o log deve mostrar:

`E-mail SMTP ativo: smtp.gmail.com:587 (StartTls), remetente ...`

Teste em `/recuperar-senha` com um usuário que tenha e-mail cadastrado (ex.: `bruno.costa`).

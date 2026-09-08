# CarWashStation

Car-wash booking system split into three .NET 10 projects so the UI and API can be deployed independently.

## Solution structure

```text
CarWashStation.Client/   Blazor WebAssembly UI (Azure Static Web Apps)
CarWashStation.Api/      ASP.NET Core Web API, EF Core, and email (Azure App Service)
CarWashStation.Shared/   Models and API contracts shared by Client and API
```

Both executable projects reference `CarWashStation.Shared`; the client calls the API with `HttpClient`.

## Local configuration

Do not put credentials in `appsettings.json`. Configure the API with user-secrets or environment variables:

```powershell
dotnet user-secrets init --project CarWashStation.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_SQL_CONNECTION" --project CarWashStation.Api
dotnet user-secrets set "Admin:Email" "YOUR_ADMIN_EMAIL" --project CarWashStation.Api
dotnet user-secrets set "Admin:Password" "YOUR_ADMIN_PASSWORD" --project CarWashStation.Api
dotnet user-secrets set "EmailSettings:SmtpUser" "YOUR_SMTP_USER" --project CarWashStation.Api
dotnet user-secrets set "EmailSettings:SmtpPass" "YOUR_SMTP_PASSWORD" --project CarWashStation.Api
```

Set `CarWashStation.Client/wwwroot/appsettings.json` → `ApiBaseUrl` to the API URL. Add the client URL to `Cors:AllowedOrigins` in the API settings.

Run the API and client in separate terminals:

```powershell
dotnet run --project CarWashStation.Api
dotnet run --project CarWashStation.Client
```

## Azure deployment

### API: Azure App Service

Publish `CarWashStation.Api/CarWashStation.Api.csproj`. In App Service Configuration, add:

- `ConnectionStrings__DefaultConnection`
- `Admin__Email` and `Admin__Password`
- `EmailSettings__SmtpUser`, `EmailSettings__SmtpPass`, and `EmailSettings__FromEmail`
- `Cors__AllowedOrigins__0=https://YOUR-STATIC-APP.azurestaticapps.net`
- Another CORS entry for the custom UI domain

Use an Azure SQL connection string; Windows integrated authentication from the old local configuration will not work in App Service.

### UI: Azure Static Web Apps (Free)

Build and deploy `CarWashStation.Client`. Publish with `dotnet publish CarWashStation.Client -c Release`; deploy the generated `publish/wwwroot` content. Before publishing, set `ApiBaseUrl` to the HTTPS App Service URL.

`staticwebapp.config.json` supplies the SPA navigation fallback for Blazor routes. Static Web Apps provides managed TLS and custom-domain support, subject to Azure's current plan limits.

## Build

```powershell
dotnet build CarWashStation.slnx
```

The existing EF Core migrations remain in `CarWashStation.Api/Migrations`.

Booking dates are calendar days in Stockholm, serialized as `yyyy-MM-dd`. Older
clients sending ISO timestamps with offsets are supported without shifting the day.
Availability and past-time validation use `Europe/Stockholm`, including daylight saving.
Run the date regression checks with:

```powershell
dotnet run --project tests/BookingDateRegression
```

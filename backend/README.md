# Backend API

.NET 8 Web API following organization standards.

## Building

```bash
dotnet restore
dotnet build
```

## Running

Configure a SQL Server connection string. Environment variables override the
development default without storing credentials in source control:

```bash
export ConnectionStrings__DefaultConnection='Server=localhost;Database=OrganizationStarter;User Id=...;Password=...;TrustServerCertificate=true'
```

Restore the repository-local EF Core tool and apply migrations before starting
a new application version:

```bash
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/Api --startup-project src/Api
```

Then start the API:

```bash
dotnet run --project src/Api
```

API available at `http://localhost:5000` with Swagger at `/swagger`.

## Testing

```bash
dotnet test
```

Tests replace SQL Server with a uniquely named EF Core InMemory database.

## User Preferences API

`GET /v1/user-preferences` returns saved preferences or these defaults:

```json
{
  "displayTheme": "light",
  "emailNotificationIndicator": true,
  "inAppNotificationIndicator": true
}
```

`PUT /v1/user-preferences` accepts the same fields. `displayTheme` must be
`light` or `dark`. Both endpoints return the standard single-item envelope.
Notification settings are stored only; notification delivery is outside this
feature.

The API currently uses the server-configured `UserPreferences:StarterUserId`.
Clients cannot select a user. Replace this temporary identity with the
authenticated user's ID when authentication is introduced.

## Migration Deployment

Apply migrations before deploying an API version that uses preferences. The
`AddUserPreferences` migration adds a new table and does not alter existing
tables.

For recovery, roll back the application first and leave the preferences table
in place so saved data remains available for a forward deployment. If the table
must be removed, export its rows before targeting the migration that precedes
`AddUserPreferences`:

```bash
dotnet tool run dotnet-ef database update 0 --project src/Api --startup-project src/Api
```

The current migration is the initial migration, so targeting `0` removes all
schema managed by this context, including saved preferences.

## Project Layout

- `src/Api/` - Main API project
  - `Controllers/` - API controllers
  - `Data/` - EF Core context and migrations
  - `DTOs/` - Data transfer objects
  - `Services/` - Business logic layer
  - `Repositories/` - Data access layer
  - `Domain/` - Domain entities
  - `Middleware/` - Custom middleware
  - `Extensions/` - Extension methods
- `tests/Api.Tests/` - Unit and integration tests

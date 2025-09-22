# minimal-api

Minimal API project using ASP.NET Core (.NET 9), Entity Framework Core (MySQL), JWT authentication, Swagger, and Mapster.

## Features

- Minimal APIs with ASP.NET Core
- Entity Framework Core with MySQL (Pomelo)
- JWT authentication with roles (ADMINISTRATOR, EDITOR)
- Swagger UI with security lock only on protected routes
- DTO mapping with Mapster (ignoring nulls/empty on PATCH)
- Validation layer for DTOs
- Separate test project

## Structure

```
minimal-api.sln
Api/
  Program.cs
  appsettings.json
  Domain/
    DTOs/
    Entities/
    Enums/
    FormValidation/
    ModelViews/
    Services/
  Infrastructure/
    Database/
    Swagger/
Test/
```

## Getting Started

1. Install .NET 9 SDK and MySQL/MariaDB.
2. Configure `Api/appsettings.json` with your connection string and JWT settings.
3. Restore and build:
   ```
   dotnet restore
   dotnet build
   ```
4. Run the API:
   ```
   dotnet run --project Api
   ```
5. Access Swagger at [https://localhost:7226/swagger](https://localhost:7226/swagger).

## Migrations

To add and update database migrations:
```
dotnet ef migrations add MigrationName --project Api --startup-project Api
dotnet ef database update --project Api --startup-project Api
```

## Authentication

- Login via `POST /administrators/login` to receive a JWT token.
- Use the token in Swagger's "Authorize" button as `Bearer <token>`.
- Endpoints are by roles (ADMINISTRATOR, EDITOR).

## Validation

DTOs are validated using custom validation classes. Error messages are returned in a list.

## Swagger Security

Custom OperationFilter ensures only endpoints requiring authorization show the lock icon.

## Mapster

Used for mapping DTOs to entities, especially for PATCH operations, ignoring null and empty values.

## Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| Unknown column 'a.Profile' | Migration missing | Add migration and update DB |
| invalid_token (audience empty) | JWT config missing | Set Issuer/Audience in config and token |
| Lock on all routes | Global security requirement | Use OperationFilter for per-route security |
| PATCH overwrites fields | DTO sends null/empty | Configure Mapster to ignore null/empty |
| Repeated validation errors | Static validation state | Instantiate validation result per request |

---

Projeto minimalista de API com ASP.NET Core, EF Core, JWT, Swagger e Mapster.  
Para instruções em português, consulte os comentários acima ou traduza este README.

---

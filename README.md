# Library Management (ASP.NET Core + EF Core)

This is a starter project for a **Library Management** system.

## Project structure

- `Library/Domain/Entities`: domain entities (Person, Account, Role, Book, AvailableBook, Publisher, Category, Reservation, Borrow)
- `Library/Infrastructure/Persistence`: EF Core `LibraryDbContext` + migrations

## Running the API

```bash
dotnet run --project .\Library\Library.csproj
```

Swagger runs in Development at `/swagger`.

## Database (SQL Server / LocalDB)

Connection strings:
- `Library/appsettings.json`: `Server=(localdb)\MSSQLLocalDB;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True`
- `Library/appsettings.Development.json`: `Server=(localdb)\MSSQLLocalDB;Database=LibraryManagementDb.Dev;Trusted_Connection=True;TrustServerCertificate=True`

## EF Core migrations

This repo pins the EF tooling locally (recommended):

```bash
cd .\Library
dotnet tool restore
```

Create a migration:

```bash
dotnet ef migrations add <Name> -p .\Library.csproj -s .\Library.csproj -o Infrastructure\Persistence\Migrations
```

Apply migrations:

```bash
dotnet ef database update -p .\Library.csproj -s .\Library.csproj
```



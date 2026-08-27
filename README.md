# Hotel Listing API

## Summary
A hotel management API built with ASP.NET Core and SQL Server.

## Features
- JWT authentication
- Role-based authorization
- In-memory caching with cache invalidation
- API rate limiting
- Integration testing with xUnit and WebApplicationFactory
- Health checks
- Structured logging

## Tech Stack

- C#
- ASP.NET Core
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT
- xUnit
- Serilog

## Running Locally

### Requirements

- .NET SDK
- SQL Server

#### Clone the repository:
```bash
git clone <repository-url>
cd <repository-folder>
```
#### Restore Dependencies and Configure Secrets
```bash
dotnet restore
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
```
#### Migrations
```bash
dotnet ef migrations add [migration name] --project HotelListing.Api.Domain --startup-project HotelListing.Api 
dotnet ef database update --project HotelListing.Api.Domain --startup-project HotelListing.Api
```


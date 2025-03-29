Run this commands:
- For creating migrations: `dotnet ef migrations add InitialCreate --project Movies.Data --startup-project Movies.Api`
- For updating migrations: `dotnet ef database update --project Movies.Data --startup-project Movies.Api`
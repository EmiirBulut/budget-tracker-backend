using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BudgetTracker.Infrastructure.Persistence;

/// <summary>
/// Used only by EF Core tooling (dotnet ef migrations). Not used at runtime.
/// Run from the Backend/ folder:
///   dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/Api
/// Update the connection string below to match your local dev database before running migrations.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("BUDGETTRACKER_CONNECTION_STRING")
            ?? "Host=localhost;Database=budgettracker_dev;Username=postgres;Password=devpass";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}

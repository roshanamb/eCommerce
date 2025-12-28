using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Data;

//docker pull postgres:latest
//docker run -d --name postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=Admin123! -p 5432:5432 postgres:latest

//dotnet ef migrations add InitPostgres -p ECommerce.Infrastructure -s ECommerce.API --context PostgresDbContext --output-dir "Migrations/PostgreSQL"
//dotnet ef database update -p ECommerce.Infrastructure -s ECommerce.API --context PostgresDbContext

public class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
    public PostgresDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ECommerce.API");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("PostgreSQL");

        var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PostgresDbContext(optionsBuilder.Options);
    }
}

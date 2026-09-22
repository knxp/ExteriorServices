using ExteriorServices.Application.Customers;
using ExteriorServices.Application.Jobs;
using ExteriorServices.Application.Properties;
using ExteriorServices.Infrastructure.Data;
using ExteriorServices.Infrastructure.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExteriorServices.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<Application.Dashboard.IDashboardRepository, DashboardRepository>();

        var connectionString = ResolveConnectionString(configuration);

        services.AddDbContext<ExteriorServicesDbContext>(options =>
        {
            var normalizedConnectionString = NormalizeExplicitSqliteConnectionString(connectionString);

            if (LooksLikeSqlite(normalizedConnectionString))
            {
                options.UseSqlite(normalizedConnectionString);
                return;
            }

            options.UseSqlServer(normalizedConnectionString);
        });

        return services;
    }

    public static string ResolveConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "The database connection string is required. Set ConnectionStrings__DefaultConnection in environment variables or Azure Key Vault.");
        }

        if (LooksLikeLocalDb(connectionString) && !IsLocalDbInstalled())
        {
            return "Data Source=ExteriorServices.db";
        }

        return connectionString;
    }

    private static string NormalizeExplicitSqliteConnectionString(string connectionString)
    {
        if (!LooksLikeSqlite(connectionString))
        {
            return connectionString;
        }

        try
        {
            var builder = new SqliteConnectionStringBuilder(connectionString);
            if (!string.IsNullOrWhiteSpace(builder.DataSource) && !Path.IsPathRooted(builder.DataSource))
            {
                var apiProjectDirectory = FindApiProjectDirectory();
                builder.DataSource = Path.GetFullPath(Path.Combine(apiProjectDirectory, builder.DataSource));
            }

            return builder.ToString();
        }
        catch
        {
            return connectionString;
        }
    }

    private static bool LooksLikeSqlite(string connectionString)
    {
        return connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains("Filename=", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeLocalDb(string connectionString)
    {
        return connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains("MSSQLLocalDB", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsLocalDbInstalled()
    {
        var candidatePaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SQL Server", "Local DB", "Binn", "SqlLocalDB.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft SQL Server", "Local DB", "Binn", "SqlLocalDB.exe")
        };

        return candidatePaths.Any(File.Exists);
    }

    private static string FindApiProjectDirectory()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var apiProjectDirectory = Path.Combine(current.FullName, "src", "ExteriorServices.Api");
            if (Directory.Exists(apiProjectDirectory))
            {
                return apiProjectDirectory;
            }

            current = current.Parent;
        }

        return Directory.GetCurrentDirectory();
    }
}
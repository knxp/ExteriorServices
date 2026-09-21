using ExteriorServices.Application.Configuration;
using ExteriorServices.Infrastructure;
using ExteriorServices.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class AppConfigurationValidatorTests
{
    [Fact]
    public void Validate_Throws_WhenConnectionStringIsMissing()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = null
            })
            .Build();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            AppConfigurationValidator.Validate(configuration));

        Assert.Contains("ConnectionStrings:DefaultConnection", exception.Message);
    }

    [Fact]
    public void Validate_Passes_WhenConnectionStringIsPresent()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=ExteriorServices;Trusted_Connection=True;TrustServerCertificate=True;"
            })
            .Build();

        var exception = Record.Exception(() => AppConfigurationValidator.Validate(configuration));

        Assert.Null(exception);
    }

    [Fact]
    public void AddInfrastructure_UsesSqlite_WhenConnectionStringIsSqlite()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = new SqliteConnectionStringBuilder
                {
                    DataSource = "ExteriorServicesTest.db"
                }.ToString()
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<DbContextOptions<ExteriorServicesDbContext>>();

        var sqliteExtension = options.Extensions
            .SingleOrDefault(extension => extension.GetType().Name == "SqliteOptionsExtension");

        Assert.NotNull(sqliteExtension);
    }

    [Fact]
    public void AddInfrastructure_UsesSqlServer_WhenLocalDbIsInstalled_ElseSqlite()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\MSSQLLocalDB;Database=ExteriorServices;Trusted_Connection=True;TrustServerCertificate=True;"
            })
            .Build();

        var resolved = DependencyInjection.ResolveConnectionString(configuration);

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<DbContextOptions<ExteriorServicesDbContext>>();

        var sqlServerExtension = options.Extensions
            .SingleOrDefault(extension => extension.GetType().Name == "SqlServerOptionsExtension");
        var sqliteExtension = options.Extensions
            .SingleOrDefault(extension => extension.GetType().Name == "SqliteOptionsExtension");

        if (resolved.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
        {
            Assert.NotNull(sqliteExtension);
            Assert.Null(sqlServerExtension);
            return;
        }

        Assert.NotNull(sqlServerExtension);
        Assert.Null(sqliteExtension);
    }

    [Fact]
    public void ResolveConnectionString_FallsBackToSqlite_WhenLocalDbIsUnavailable()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\MSSQLLocalDB;Database=ExteriorServices;Trusted_Connection=True;TrustServerCertificate=True;"
            })
            .Build();

        var resolved = DependencyInjection.ResolveConnectionString(configuration);

        Assert.Contains("Data Source=", resolved, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("MSSQLLocalDB", resolved, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddInfrastructure_ResolvesRelativeSqliteConnectionString_ToStableAbsolutePath()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data Source=ExteriorServices.db"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<DbContextOptions<ExteriorServicesDbContext>>();

        var sqliteExtension = options.Extensions
            .SingleOrDefault(extension => extension.GetType().Name == "SqliteOptionsExtension");

        Assert.NotNull(sqliteExtension);

        var connectionString = sqliteExtension!
            .GetType()
            .GetProperty("ConnectionString")?
            .GetValue(sqliteExtension) as string;

        Assert.False(string.IsNullOrWhiteSpace(connectionString));
        Assert.Contains("ExteriorServices.db", connectionString);
        Assert.True(Path.IsPathRooted(new SqliteConnectionStringBuilder(connectionString!).DataSource));
    }
}

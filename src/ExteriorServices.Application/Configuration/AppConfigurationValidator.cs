using Microsoft.Extensions.Configuration;

namespace ExteriorServices.Application.Configuration;

public static class AppConfigurationValidator
{
    public static void Validate(IConfiguration configuration)
    {
        EnsureRequiredValue(
            configuration,
            "ConnectionStrings:DefaultConnection",
            "The database connection string is required. Set ConnectionStrings__DefaultConnection in environment variables or Azure Key Vault.");
    }

    private static void EnsureRequiredValue(
        IConfiguration configuration,
        string key,
        string message)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{message} Missing configuration value: {key}");
        }
    }
}

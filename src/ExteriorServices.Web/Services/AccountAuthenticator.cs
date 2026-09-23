using ExteriorServices.Web.Models;
using Microsoft.Extensions.Options;

namespace ExteriorServices.Web.Services;

public sealed class AccountAuthenticator : IAccountAuthenticator
{
    public const string AdminRole = "Admin";
    public const string TestRole = "Test";

    private readonly AuthOptions _options;

    public AccountAuthenticator(IOptions<AuthOptions> options)
    {
        _options = options.Value;
    }

    public SiteAccount? Validate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        if (string.Equals(username, _options.AdminUsername, StringComparison.OrdinalIgnoreCase)
            && SitePasswordHasher.Verify(password, _options.AdminPasswordHash))
        {
            return new SiteAccount(_options.AdminUsername, AdminRole);
        }

        if (string.Equals(username, _options.TestUsername, StringComparison.OrdinalIgnoreCase)
            && SitePasswordHasher.Verify(password, _options.TestPasswordHash))
        {
            return new SiteAccount(_options.TestUsername, TestRole);
        }

        return null;
    }
}

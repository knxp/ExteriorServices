namespace ExteriorServices.Web.Services;

public interface IAccountAuthenticator
{
    SiteAccount? Validate(string username, string password);
}

public sealed record SiteAccount(string Username, string Role);

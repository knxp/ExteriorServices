namespace ExteriorServices.Web.Models;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string AdminUsername { get; set; } = string.Empty;

    public string AdminPasswordHash { get; set; } = string.Empty;

    public string TestUsername { get; set; } = string.Empty;

    public string TestPasswordHash { get; set; } = string.Empty;
}

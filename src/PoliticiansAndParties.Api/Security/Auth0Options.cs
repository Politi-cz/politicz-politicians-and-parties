namespace PoliticiansAndParties.Api.Security;

public class Auth0Options
{
    public const string Auth0 = "Auth0";

    public string Domain { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public IEnumerable<string> Permissions { get; set; } = Enumerable.Empty<string>();
}

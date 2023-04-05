namespace PoliticiansAndParties.Api.Test.Integration.Handlers;

public class TestAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public const string AuthenticationScheme = "Test";

    public IEnumerable<string> AllowedScopes { get; set; } = Enumerable.Empty<string>();
}

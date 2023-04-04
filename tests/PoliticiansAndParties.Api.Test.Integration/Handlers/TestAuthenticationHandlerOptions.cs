namespace PoliticiansAndParties.Api.Test.Integration.Handlers;

public class TestAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public IEnumerable<string> AllowedScopes { get; set; } = Enumerable.Empty<string>();
}

namespace PoliticiansAndParties.Api.Test.Integration.Handlers;

public class TestAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public IEnumerable<string> Permissions { get; set; } = Enumerable.Empty<string>();
}

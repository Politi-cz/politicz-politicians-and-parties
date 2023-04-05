namespace PoliticiansAndParties.Api.Test.Integration.Handlers;

public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationHandlerOptions>
{
    public const string AuthenticationScheme = "Test";
    public const string TestScope = "TestScope";
    private readonly IEnumerable<string> _predefinedScopes;

    public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) => _predefinedScopes = options.CurrentValue.AllowedScopes;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            Context.Request.Headers.TryGetValue(TestScope, out var testScope)
            ? new Claim("scope", testScope!)
            : new Claim("scope", string.Join(" ", _predefinedScopes)),
            new Claim("issuer", "fads"),
        };
        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var result = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(result));
    }
}

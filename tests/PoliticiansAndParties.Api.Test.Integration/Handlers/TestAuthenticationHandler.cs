namespace PoliticiansAndParties.Api.Test.Integration.Handlers;

public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationHandlerOptions>
{
    public const string AuthenticationScheme = "Test";
    public const string TestPermission = "TestScope";
    private readonly IEnumerable<string> _predefinedPermissions;

    public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) => _predefinedPermissions = options.CurrentValue.Permissions;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            Context.Request.Headers.TryGetValue(TestPermission, out var testPermission)
            ? new Claim("permissions", testPermission!)
            : new Claim("permissions", string.Join(" ", _predefinedPermissions)),
        };
        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var result = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(result));
    }
}

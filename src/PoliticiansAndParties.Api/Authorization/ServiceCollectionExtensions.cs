namespace PoliticiansAndParties.Api.Authorization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Authentication(this IServiceCollection serviceCollection, string audience, string authority)
    {
        _ = serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                };
            });

        return serviceCollection;
    }

    public static IServiceCollection AddAuth0Authorization(this IServiceCollection serviceCollection, string authority)
        => serviceCollection
            .AddAuthorization(options => options.AddPolicy(
              AuthConstants.ModifyPolicy,
              policy => policy.Requirements.Add(
                  new HasScopeRequirement(AuthConstants.ModifyScopeName, authority))))
            .AddSingleton<IAuthorizationHandler, HasScopeHandler>();
}

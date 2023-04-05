using PoliticiansAndParties.Api.Security.Handlers;

namespace PoliticiansAndParties.Api.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Security(this IServiceCollection serviceCollection, string audience, string authority)
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

        return serviceCollection
            .AddAuthorization(options => options.AddPolicy(
                SecurityConstants.ModifyPolicy,
                policy => policy.Requirements.Add(
                        new HasScopeRequirement(SecurityConstants.ModifyScopeName, authority))))
            .AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    }
}

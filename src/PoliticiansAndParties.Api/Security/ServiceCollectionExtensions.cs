namespace PoliticiansAndParties.Api.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Security(this IServiceCollection serviceCollection, Auth0Options auth0Options)
    {
        string authority = $"https://{auth0Options.Domain}/";

        _ = serviceCollection.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });

        _ = serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = auth0Options.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                };
            });

        return serviceCollection
            .AddAuthorization(options =>
            {
                foreach (string permission in auth0Options.Permissions)
                {
                    options.AddPolicy(permission, policy => policy.RequireClaim("permissions", permission));
                }
            });
    }
}

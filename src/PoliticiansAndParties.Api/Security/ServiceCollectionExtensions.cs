namespace PoliticiansAndParties.Api.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Security(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var auth0Options = configuration.GetSection("Auth0").Get<Auth0Options>();

        string authority = $"https://{auth0Options.Domain}/";

        _ = serviceCollection.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
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
            .AddAuthorization(options => options.AddPolicy("modify:parties-politicians", policy =>
            {
                _ = policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                _ = policy.RequireClaim("permissions", "modify:parties-politicians");
            }));
    }
}

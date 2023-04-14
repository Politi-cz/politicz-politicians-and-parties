namespace PoliticiansAndParties.Api.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        => serviceCollection
            .AddScoped<IPoliticianService, PoliticianService>()
            .AddScoped<IPoliticalPartyService, PoliticalPartyService>();
}

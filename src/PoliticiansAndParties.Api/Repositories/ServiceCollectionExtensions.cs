namespace PoliticiansAndParties.Api.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        => serviceCollection
            .AddScoped<IPoliticianRepository, PoliticianRepository>()
            .AddScoped<IPoliticalPartyRepository, PoliticalPartyRepository>();
}

namespace PoliticiansAndParties.Api.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>()
            .AddSingleton<DatabaseInitializer>();
}

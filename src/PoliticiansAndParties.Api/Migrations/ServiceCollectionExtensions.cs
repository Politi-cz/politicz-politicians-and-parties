namespace PoliticiansAndParties.Api.Migrations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigrator(this IServiceCollection serviceCollection) =>
        serviceCollection.AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddSqlServer()
                .WithGlobalConnectionString(sp =>
                {
                    var dbOptions = sp.GetRequiredService<IOptions<PoliticiansPartiesOptions>>().Value.Database;
                    return dbOptions.DefaultConnection;
                })
                .ScanIn(Assembly.GetExecutingAssembly()).For.All());
}

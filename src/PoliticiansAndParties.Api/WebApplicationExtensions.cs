namespace PoliticiansAndParties.Api;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        var dbOptions = scope.ServiceProvider.GetRequiredService<IOptions<PoliticiansPartiesOptions>>()
            .Value.Database;
        await databaseInitializer.Initialize(dbOptions.Name);

        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        runner.ListMigrations();
        runner.MigrateUp();

        return app;
    }
}

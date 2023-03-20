namespace PoliticiansAndParties.Api.Test.Integration;

// TODO: In the future instead of creating a docker container for each test class create collection fixture
// for each controller. So for example PoliticalPartyController will have one collection fixture running only 1 docker container
// for all test classes related to political party controller. This aproach needs to implement Respawner to reset DB state
// after each test. Inspire from Nick Chapsas Respawner video
public class PoliticiansAndPartiesApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly TestcontainerDatabase _dbContainer;

    public PoliticiansAndPartiesApiFactory() =>
        _dbContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration { Password = "Password12345" })
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

    public async Task InitializeAsync() => await _dbContainer.StartAsync();

    async Task IAsyncLifetime.DisposeAsync() => await _dbContainer.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.ConfigureLogging(logging => _ = logging.ClearProviders());

        // TODO Use sql connection string builder or something like that
        string? masterConnectionString = _dbContainer.ConnectionString;
        masterConnectionString += "TrustServerCertificate=True;";
        string defaultConnectionString =
            masterConnectionString.Replace("master", "politicz-politicians-and-parties");

        _ = builder.ConfigureTestServices(services =>
        {
            _ = services.RemoveAll(typeof(IDbConnectionFactory));
            _ = services.AddSingleton<IDbConnectionFactory>(_ =>
                new SqlServerConnectionFactory(new ConnectionStrings(
                    masterConnectionString,
                    defaultConnectionString)));

            _ = services.RemoveAll(typeof(IMigrationProcessor));
            _ = services.AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c.AddSqlServer()
                    .WithGlobalConnectionString(defaultConnectionString)
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());
        });
    }
}

using Microsoft.Data.SqlClient;
using Respawn;
using System.Data.Common;

namespace PoliticiansAndParties.Api.Test.Integration;

public class PoliticiansAndPartiesApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration { Password = "Password12345" })
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithCleanUp(true)
        .Build();

    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;
    private string _masterConnectionString = default!;
    private string _defaultConnectionString = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    public async Task ResetDatabase() => await _respawner.ResetAsync(_dbConnection);

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // TODO Use sql connection string builder or something like that
        _masterConnectionString = _dbContainer.ConnectionString + "TrustServerCertificate=True;";
        _defaultConnectionString = _masterConnectionString.Replace("master", "politicz-politicians-and-parties");

        _dbConnection = new SqlConnection(_defaultConnectionString);
        HttpClient = CreateClient();
        await InitializeRespawner();
    }

    async Task IAsyncLifetime.DisposeAsync() => await _dbContainer.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.ConfigureLogging(logging => _ = logging.ClearProviders());

        _ = builder.ConfigureTestServices(services =>
        {
            _ = services.RemoveAll(typeof(IDbConnectionFactory));
            _ = services.AddSingleton<IDbConnectionFactory>(_ =>
                new SqlServerConnectionFactory(new ConnectionStrings(
                    _masterConnectionString,
                    _defaultConnectionString)));

            _ = services.RemoveAll(typeof(IMigrationProcessor));
            _ = services.AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c.AddSqlServer()
                    .WithGlobalConnectionString(_defaultConnectionString)
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());
        });
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = new[] { "dbo" },
            });
    }
}

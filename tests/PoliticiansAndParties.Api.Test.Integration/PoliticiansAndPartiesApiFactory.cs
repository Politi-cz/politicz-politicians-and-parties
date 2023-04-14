using PoliticiansAndParties.Api.Database;
using PoliticiansAndParties.Api.Options;

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

    public HttpClient UnauthorizedClient { get; private set; } = default!;

    public async Task ResetDatabase() => await _respawner.ResetAsync(_dbConnection);

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var connBuilder = new SqlConnectionStringBuilder(_dbContainer.ConnectionString)
        {
            TrustServerCertificate = true,
        };
        _masterConnectionString = connBuilder.ConnectionString;
        connBuilder.InitialCatalog = "politicz-politicians-and-parties";
        _defaultConnectionString = connBuilder.ConnectionString;

        _dbConnection = new SqlConnection(_defaultConnectionString);
        UnauthorizedClient = CreateClient();
        HttpClient = CreateClient();
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: TestAuthenticationHandler.AuthenticationScheme);
        await InitializeRespawner();
    }

    async Task IAsyncLifetime.DisposeAsync() => await _dbContainer.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.ConfigureLogging(logging => _ = logging.ClearProviders());

        _ = builder.ConfigureTestServices(services =>
        {
            _ = services.Configure<TestAuthenticationHandlerOptions>(o =>
                o.Permissions = new[] { "modify:parties-politicians" });

            _ = services.Configure<PoliticiansPartiesOptions>(o => o.Database = new DatabaseOptions
            {
                DefaultConnection = _defaultConnectionString,
                MasterConnection = _masterConnectionString,
                Name = "politicz-politicians-and-parties",
            });

            _ = services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationScheme;
                    o.DefaultScheme = TestAuthenticationHandler.AuthenticationScheme;
                    o.DefaultChallengeScheme = TestAuthenticationHandler.AuthenticationScheme;
                })
                .AddScheme<TestAuthenticationHandlerOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.AuthenticationScheme, _ => { });

            _ = services.RemoveAll(typeof(IDbConnectionFactory));
            _ = services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();

            _ = services.RemoveAll(typeof(IMigrationProcessor));
            _ = services.AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c.AddSqlServer()
                    .WithGlobalConnectionString(sp =>
                    {
                        var dbOptions = sp.GetRequiredService<IOptions<PoliticiansPartiesOptions>>().Value.Database;
                        return dbOptions.DefaultConnection;
                    })
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

            _ = services.AddAuthorization(con =>
                con.AddPolicy("modify:parties-politicians", p
                    =>
                {
                    _ = p.AddAuthenticationSchemes(TestAuthenticationHandler.AuthenticationScheme);
                    _ = p.RequireClaim("permissions", "modify:parties-politicians");
                }));
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

// TODO: Change it to logger configuration in appsettings.json
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("PoliticalPartiesApi_");
builder.Host.UseSerilog();

// Configure authentication and authorization
builder.Services.AddAuth0Security(builder.Configuration.GetSection(Auth0Options.Auth0).Get<Auth0Options>()!);

// Add services to the container.
// TODO: Add secret manager for a local development
builder.Services.AddSingleton<IDbConnectionFactory>(new SqlServerConnectionFactory(
    new ConnectionStrings(
        builder.Configuration.GetConnectionString("MasterConnection")!,
        builder.Configuration.GetConnectionString("DefaultConnection")!)));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<IPoliticianRepository, PoliticianRepository>();
builder.Services.AddScoped<IPoliticianService, PoliticianService>();
builder.Services.AddScoped<IPoliticalPartyRepository, PoliticalPartyRepository>();
builder.Services.AddScoped<IPoliticalPartyService, PoliticalPartyService>();
builder.Services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

// TODO Add all validations through an extension method RegisterValidators or something like that.
builder.Services.AddScoped<IValidator<PoliticianRequest>, PoliticianRequestValidator>();
builder.Services.AddScoped<IValidator<PoliticalPartyRequest>, PoliticalPartyRequestValidator>();

builder.Services.AddLogging(c => c.AddFluentMigratorConsole())
    .AddFluentMigratorCore()
    .ConfigureRunner(c => c.AddSqlServer()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(Assembly.GetExecutingAssembly()).For.All());

builder.Services.AddCors();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(
        new ProducesResponseTypeAttribute(typeof(ErrorDetail), (int)HttpStatusCode.BadRequest));
    options.Filters.Add(
        new ProducesResponseTypeAttribute(typeof(ErrorDetail), (int)HttpStatusCode.NotFound));
    options.Filters.Add(
        new ProducesResponseTypeAttribute(typeof(ErrorDetail), (int)HttpStatusCode.InternalServerError));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s => s.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "Politicians and political parties",
    Version = "v1",
    Description = "API providing CRUD operations for politicians and political parties",
    Contact = new OpenApiContact { Url = new Uri("https://github.com/PetrKoller") },
}));

var app = builder.Build();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();

// TODO fix for testing, now hardcoded, need to find a way in PoliticiansAndPartiesFactory to get database initialized and initialize with own database name
// now struggling to create scope and get required service in test configuration
await databaseInitializer.Initialize("politicz-politicians-and-parties");

using var scope = app.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

runner.ListMigrations();
runner.MigrateUp();

app.Run();

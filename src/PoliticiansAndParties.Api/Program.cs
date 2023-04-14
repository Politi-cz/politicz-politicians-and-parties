using PoliticiansAndParties.Api;

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
builder.Services.Configure<PoliticiansPartiesOptions>(builder.Configuration)
    .AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>))
    .AddAuth0Security(builder.Configuration)
    .AddDatabase()
    .AddMigrator()
    .AddRepositories()
    .AddServices()
    .AddValidatorsFromAssemblyContaining<PoliticalPartyRequestValidator>()
    .AddCors()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(s => s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Politicians and political parties",
        Version = "v1",
        Description = "API providing CRUD operations for politicians and political parties",
        Contact = new OpenApiContact { Url = new Uri("https://github.com/PetrKoller") },
    }));

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

var app = builder.Build();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
    .UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();
await app.MigrateDatabase();

await app.RunAsync();

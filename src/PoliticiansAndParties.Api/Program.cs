using FluentMigrator.Runner;
using FluentValidation;
using PoliticiansAndParties.Api.Contracts.Requests;
using PoliticiansAndParties.Api.Database;
using PoliticiansAndParties.Api.Logging;
using PoliticiansAndParties.Api.Middleware;
using PoliticiansAndParties.Api.Repositories;
using PoliticiansAndParties.Api.Services;
using PoliticiansAndParties.Api.Validators;
using Serilog;
using Serilog.Events;
using System.Reflection;

// TODO: Change it to logger configuration in appsetings.json
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("PoliticalPartiesApi_");
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddSingleton<IDbConnectionFactory>(new SqlServerConnectionFactory(
    new ConnectionStrings(builder.Configuration.GetConnectionString("MasterConnection"),
        builder.Configuration.GetConnectionString("DefaultConnection"))));
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
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync(builder.Configuration.GetValue<string>("Database"));

using var scope = app.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

runner!.ListMigrations();
runner.MigrateUp();

app.Run();
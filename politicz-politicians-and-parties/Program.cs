using FluentMigrator.Runner;
using FluentValidation;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Dtos;
using politicz_politicians_and_parties.Logging;
using politicz_politicians_and_parties.Middleware;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Services;
using politicz_politicians_and_parties.Validators;
using Serilog;
using Serilog.Events;
using System.Reflection;


// TODO: Change it to logger configuration in appsetings.json
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddSingleton<IDbConnectionFactory>(new SqlServerConnectionFactory(
    new ConnectionStrings(builder.Configuration.GetConnectionString("MasterConnection"), builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<IPoliticianRepository, PoliticianRepository>();
builder.Services.AddScoped<IPoliticianService, PoliticianService>();
builder.Services.AddScoped<IPoliticalPartyRepository, PoliticalPartyRepository>();
builder.Services.AddScoped<IPoliticalPartyService, PoliticalPartyService>();
builder.Services.AddTransient(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

// TODO Add all validations through an extension method RegisterValidators or something like that. 
builder.Services.AddScoped<IValidator<PoliticianDto>, PoliticianDtoValidator>();
builder.Services.AddScoped<IValidator<PoliticalPartyDto>, PoliticalPartyDtoValidator>();

builder.Services.AddLogging(c => c.AddFluentMigratorConsole())
    .AddFluentMigratorCore()
    .ConfigureRunner(c => c.AddSqlServer()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(Assembly.GetExecutingAssembly()).For.All());


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


// app.ConfigureExceptionHandler(scope.ServiceProvider.GetRequiredService<ILoggerAdapter<object>>());
app.UseHttpsRedirection();


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

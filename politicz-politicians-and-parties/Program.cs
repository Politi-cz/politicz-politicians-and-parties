using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using politicz_politicians_and_parties.Database;
using politicz_politicians_and_parties.Extensions;
using politicz_politicians_and_parties.Models;
using politicz_politicians_and_parties.Repositories;
using politicz_politicians_and_parties.Services;
using System.Data;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IDbConnectionFactory>(new SqlServerConnectionFactory(
    new ConnectionStrings(builder.Configuration.GetConnectionString("MasterConnection"), builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<IPoliticianRepository, PoliticianRepository>();
builder.Services.AddScoped<IPoliticianService, PoliticianService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else { 
    app.ConfigureExceptionHandler();
}


app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync(builder.Configuration.GetValue<string>("Database"));

app.Run();

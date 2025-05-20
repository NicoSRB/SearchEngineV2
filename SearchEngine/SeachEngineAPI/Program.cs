using Microsoft.EntityFrameworkCore;
using SeachEngineAPI.Interfaces;
using SeachEngineAPI.Services;
using StackExchange.Redis;
using SeachEngineAPI.DbContexts;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Diagnostics.Metrics;
using NLog;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile().GetCurrentClassLogger();
logger.Info("Starting Test");

var builder = WebApplication.CreateBuilder(args);
var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddDbContext<postgreDbContext>(options =>
    options.UseNpgsql(postgresConnectionString)
);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers and DbContext
builder.Services.AddDbContext<postgreDbContext>(options =>
    options.UseNpgsql(postgresConnectionString)
);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "SampleInstance";
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse("localhost:6379", true);
    config.AbortOnConnectFail = false; // prevents crashing on startup if Redis is slow
    return ConnectionMultiplexer.Connect(config);
});

//// Add OpenTele
//builder.Services.AddOpenTelemetry()
//    .WithMetrics(metrics =>
//    {
//        metrics
//            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SearchEngineAPI"))
//            .AddMeter("SearchEngineAPI") // <-- name used in your controller
//            .AddAspNetCoreInstrumentation();
//    });

builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ITermnetClient, TermnetClient>();
builder.Services.AddScoped<ICacheService, SearchCacheService>();
builder.Services.AddScoped<ISearchRepository, PostgresSearchRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()  // Allow any origin
               .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
               .AllowAnyHeader(); // Allow any headers
    });
});

builder.Services.AddHttpClient<ITermnetClient, TermnetClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5150/"); // Adjust the base address as needed
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

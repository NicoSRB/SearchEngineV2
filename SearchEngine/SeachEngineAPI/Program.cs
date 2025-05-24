using Microsoft.EntityFrameworkCore;
using SeachEngineAPI.Interfaces;
using SeachEngineAPI.Services;
using StackExchange.Redis;
using SeachEngineAPI.DbContexts;
using NLog;
using SeachEngineAPI.Repositories;
using Shared;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile().GetCurrentClassLogger();
logger.Info("Starting Test");

var builder = WebApplication.CreateBuilder(args);
var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");
var mongoConnectionString = builder.Configuration.GetConnectionString("Mongo"); 

builder.Services.AddDbContext<postgreDbContext>(options =>
    options.UseNpgsql(postgresConnectionString)
);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers and DbContext
string backend = builder.Configuration["Backend"] ?? "Postgres";

if (backend == "Postgres")
{
    builder.Services.AddScoped<ISearchRepository, PostgresSearchRepository>();
    builder.Services.AddDbContext<postgreDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));
}
else if (backend == "Mongo")
{
    builder.Services.AddScoped<ISearchRepository, MongoSearchRepository>();
    builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
    builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
}
    
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

builder.Services.AddSingleton<SearchMetrics>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ITermnetClient, TermnetClient>();
builder.Services.AddScoped<ICacheService, SearchCacheService>();
builder.Services.AddScoped<ISearchRepository, PostgresSearchRepository>();


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

builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation()
            .AddMeter("SearchEngineAPI")
            .AddPrometheusExporter(); 
    });
builder.Services.AddSingleton(sp => new Meter("SearchEngineAPI"));

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddPrometheusExporter();
        metrics.AddMeter("Microsoft.AspNetCore.Hosting");
        metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
        metrics.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new double[]
                {
                    0, 0.005, 0.01, 0.025, 0.05,
                    0.075, 0.1, 0.25, 0.5, 0.75,
                    1, 2.5, 5, 7.5, 10
                }
            });
    });

var app = builder.Build();

app.UseRouting();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

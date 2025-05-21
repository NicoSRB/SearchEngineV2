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
builder.Services.AddDbContext<postgreDbContext>(options =>
    options.UseNpgsql(postgresConnectionString)
);
string backend = builder.Configuration["Backend"] ?? "postgres";

if (backend == "postgres")
{
    builder.Services.AddScoped<ISearchRepository, PostgresSearchRepository>();
    builder.Services.AddDbContext<postgreDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));
}
else if (backend == "mongo")
{
    builder.Services.AddScoped<ISearchRepository, MongoSearchRepository>();
    builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
    builder.Services.AddSingleton<MongoDbContext>();
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

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8082); // Replace with correct port
});

//// Add OpenTele  
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("SearchEngineAPI"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter("SearchEngineAPI") // Required to export custom metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

var app = builder.Build();

// Middleware for Prometheus metrics
app.UseRouting();
// Middleware to collect HTTP metrics

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapPrometheusScrapingEndpoint();
});

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

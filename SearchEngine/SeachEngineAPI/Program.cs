using SeachEngineAPI.Context;
using Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using SeachEngineAPI.Interfaces;
using SeachEngineAPI.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Ensure Paths.DATABASE is properly formatted
var connectionStringBuilder1 = new SqliteConnectionStringBuilder
{
    DataSource = Paths.DATABASEDB1
    //DataSource = Paths.DatabasePathForDocker
};

//var conntectionStringBuilder2 = new SqliteConnectionStringBuilder
//{
//    DataSource = Paths.DATABASEDB2
//};

string connectionString1 = connectionStringBuilder1.ConnectionString;
//string conntectionString2 = conntectionStringBuilder2.ConnectionString;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers and DbContext
builder.Services.AddControllers();
builder.Services.AddDbContext<SearchDb1Context>(options =>
    options.UseSqlite(connectionString1));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Adjust the Redis server address as needed
    options.InstanceName = "SampleInstance";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("redis:6379", true);
    return ConnectionMultiplexer.Connect(configuration);
});

//builder.Services.AddDbContext<SearchDb2Context>(options =>
//options.UseSqlite(conntectionString2));
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ITermnetClient, TermnetClient>();
builder.Services.AddScoped<ICacheService, SearchCacheService>();

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

//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<SearchDbContext>();
//    dbContext.Database.EnsureCreated();
//}


app.UseCors("AllowAllOrigins");
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

using SeachEngineAPI.Context;
using Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using SeachEngineAPI.Interfaces;
using SeachEngineAPI.Services;

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
//builder.Services.AddDbContext<SearchDb2Context>(options =>
    //options.UseSqlite(conntectionString2));
builder.Services.AddScoped<ISearchService, SearchService>();
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

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8082); // or 8082, etc.
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

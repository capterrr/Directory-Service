using DirectoryService.Extensions;
using DirectoryService.Infrastructure;
using DirectoryService.Storage;
using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database options
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.AddDbContext<DirectoryDbContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseOptions>>().Value;
    options.UseNpgsql(dbOptions.GetConnectionString());
});

// Add Infrastructure services (MediatR handlers)
builder.Services.AddInfrastructure();

var app = builder.Build();

// Initialize database with migrations
await app.MigrateDatabaseAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

// Initialize storage with sample data (legacy - can be removed later)
LocationStorage.InitializeStorage();
PositionStorage.InitializeStorage();

app.Run();


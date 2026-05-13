using DirectoryService.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

// Initialize storage with sample data
LocationStorage.InitializeStorage();
PositionStorage.InitializeStorage();

app.Run();


public record CreateLocationRequest(
    string Name,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Timezone);

public record CreatePositionRequest(
    string Name,
    string Description);

public record UpdateLocationRequest(
    string Name,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Timezone);

public record UpdatePositionRequest(
    string? Name,
    string? Description);

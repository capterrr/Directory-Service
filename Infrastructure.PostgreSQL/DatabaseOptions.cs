namespace Infrastructure.PostgreSQL;

public class DatabaseOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = "directory_service";
    public string Username { get; set; } = "postgres";
    public string Password { get; set; } = "postgres";

    public string GetConnectionString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
    }
}
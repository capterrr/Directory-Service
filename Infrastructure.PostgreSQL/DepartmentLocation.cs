namespace Infrastructure.PostgreSQL;

public class DepartmentLocation
{
    public Guid LocationId { get; set; }
    public Guid DepartmentId { get; set; }
}
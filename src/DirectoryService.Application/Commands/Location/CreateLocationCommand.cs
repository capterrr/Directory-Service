using MediatR;

namespace DirectoryService.Application.Commands.Location;

public record CreateLocationCommand : IRequest<Domain.Entities.Location>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? Region { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public string? Timezone { get; init; }
    public Guid? DepartmentId { get; init; }
    public bool IsActive { get; init; } = true;
}
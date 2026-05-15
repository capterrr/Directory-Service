using MediatR;

namespace DirectoryService.Application.Queries.Location;

public record GetLocationByIdQuery : IRequest<Domain.Entities.Location?>
{
    public required Guid Id { get; init; }
}
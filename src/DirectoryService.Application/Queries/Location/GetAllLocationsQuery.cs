using MediatR;

namespace DirectoryService.Application.Queries.Location;

public record GetAllLocationsQuery : IRequest<IReadOnlyList<Domain.Entities.Location>>;
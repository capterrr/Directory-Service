using MediatR;

namespace DirectoryService.Application.Queries.Position;

public record GetAllPositionsQuery : IRequest<IReadOnlyList<Domain.Entities.Position>>;
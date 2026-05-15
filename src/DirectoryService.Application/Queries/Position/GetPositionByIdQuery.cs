using MediatR;

namespace DirectoryService.Application.Queries.Position;

public record GetPositionByIdQuery : IRequest<Domain.Entities.Position?>
{
    public required Guid Id { get; init; }
}
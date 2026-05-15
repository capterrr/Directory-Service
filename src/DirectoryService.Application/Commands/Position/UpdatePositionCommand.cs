using MediatR;

namespace DirectoryService.Application.Commands.Position;

public record UpdatePositionCommand : IRequest<Domain.Entities.Position>
{
    public required Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
}
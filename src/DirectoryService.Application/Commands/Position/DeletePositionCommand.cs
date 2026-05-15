using MediatR;

namespace DirectoryService.Application.Commands.Position;

public record DeletePositionCommand : IRequest<bool>
{
    public required Guid Id { get; init; }
}
using MediatR;

namespace DirectoryService.Application.Commands.Location;

public record DeleteLocationCommand : IRequest<bool>
{
    public required Guid Id { get; init; }
}
using MediatR;

namespace DirectoryService.Application.Commands.Position;

public record CreatePositionCommand : IRequest<Domain.Entities.Position>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public int? Rank { get; init; }
    public int? Depth { get; init; }
    public Guid? ParentId { get; init; }
    public Guid? DepartmentId { get; init; }
    public bool IsActive { get; init; } = true;
}
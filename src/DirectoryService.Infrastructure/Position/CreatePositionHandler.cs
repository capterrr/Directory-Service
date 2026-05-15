using Domain.Entities;
using Domain.ValueObjects;
using DirectoryService.Application.Commands.Position;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Position;

public class CreatePositionHandler : IRequestHandler<CreatePositionCommand, Domain.Entities.Position>
{
    private readonly DirectoryDbContext _context;

    public CreatePositionHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Position> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        var name = Name.Create(request.Name);

        var exists = await _context.Positions
            .AnyAsync(p => p.Name.Value == name.Value, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"Position with name '{name.Value}' already exists.");
        }

        var now = UtcDateTime.Create(DateTime.UtcNow);
        var id = EntityId.Create();
        
        var position = Domain.Entities.Position.Create(
            id,
            name,
            Description.Create(request.Description ?? string.Empty),
            IsActive.Create(request.IsActive),
            now,
            now);

        _context.Positions.Add(position);
        
        await _context.SaveChangesAsync(cancellationToken);

        return position;
    }
}
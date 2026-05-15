using Domain.Entities;
using Domain.ValueObjects;
using DirectoryService.Application.Commands.Position;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Position;

public class UpdatePositionHandler : IRequestHandler<UpdatePositionCommand, Domain.Entities.Position>
{
    private readonly DirectoryDbContext _context;

    public UpdatePositionHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Position> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id.Value == request.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Position with ID {request.Id} not found.");

        var now = UtcDateTime.Create(DateTime.UtcNow);
        
        if (!string.IsNullOrEmpty(request.Name))
        {
            position = position.UpdateName(Name.Create(request.Name), now);
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            position = position.UpdateDescription(Description.Create(request.Description), now);
        }

        _context.Positions.Update(position);
        await _context.SaveChangesAsync(cancellationToken);

        return position;
    }
}
using Domain.ValueObjects;
using DirectoryService.Application.Commands.Position;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Position;

public class DeletePositionHandler : IRequestHandler<DeletePositionCommand, bool>
{
    private readonly DirectoryDbContext _context;

    public DeletePositionHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id.Value == request.Id, cancellationToken);

        if (position == null)
        {
            return false;
        }

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
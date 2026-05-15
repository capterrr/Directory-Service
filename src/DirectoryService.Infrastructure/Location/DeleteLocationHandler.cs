using DirectoryService.Application.Commands.Location;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Location;

public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, bool>
{
    private readonly DirectoryDbContext _context;

    public DeleteLocationHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id.Value == request.Id, cancellationToken);

        if (location == null)
        {
            return false;
        }

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
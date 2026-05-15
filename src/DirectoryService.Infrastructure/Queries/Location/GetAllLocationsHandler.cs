using Domain.Entities;
using DirectoryService.Application.Queries.Location;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Queries.Location;

public class GetAllLocationsHandler : IRequestHandler<GetAllLocationsQuery, IReadOnlyList<Domain.Entities.Location>>
{
    private readonly DirectoryDbContext _context;

    public GetAllLocationsHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Domain.Entities.Location>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await _context.Locations.ToListAsync(cancellationToken);
        return locations;
    }
}
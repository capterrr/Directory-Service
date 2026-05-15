using Domain.Entities;
using DirectoryService.Application.Queries.Location;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Queries.Location;

public class GetLocationByIdHandler : IRequestHandler<GetLocationByIdQuery, Domain.Entities.Location?>
{
    private readonly DirectoryDbContext _context;

    public GetLocationByIdHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Location?> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id.Value == request.Id, cancellationToken);

        return entity;
    }
}
using Domain.Entities;
using DirectoryService.Application.Queries.Position;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Queries.Position;

public class GetAllPositionsHandler : IRequestHandler<GetAllPositionsQuery, IReadOnlyList<Domain.Entities.Position>>
{
    private readonly DirectoryDbContext _context;

    public GetAllPositionsHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Domain.Entities.Position>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
    {
        var positions = await _context.Positions.ToListAsync(cancellationToken);
        return positions;
    }
}
using Domain.Entities;
using Domain.ValueObjects;
using DirectoryService.Application.Queries.Position;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Queries.Position;

public class GetPositionByIdHandler : IRequestHandler<GetPositionByIdQuery, Domain.Entities.Position?>
{
    private readonly DirectoryDbContext _context;

    public GetPositionByIdHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Position?> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id.Value == request.Id, cancellationToken);

        return entity;
    }
}
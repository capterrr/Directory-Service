using Domain.Entities;
using Domain.ValueObjects;
using DirectoryService.Application.Commands.Location;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Location;

public class UpdateLocationHandler : IRequestHandler<UpdateLocationCommand, Domain.Entities.Location>
{
    private readonly DirectoryDbContext _context;

    public UpdateLocationHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Location> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id.Value == request.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Location with ID {request.Id} not found.");

        var now = UtcDateTime.Create(DateTime.UtcNow);

        var name = string.IsNullOrEmpty(request.Name) ? location.Name : Name.Create(request.Name);
        var timezone = string.IsNullOrEmpty(request.Timezone) ? location.Timezone : Timezone.Create(request.Timezone);

        var addressParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(request.AddressLine1))
            addressParts.Add(request.AddressLine1);
        if (!string.IsNullOrWhiteSpace(request.AddressLine2))
            addressParts.Add(request.AddressLine2);
        if (!string.IsNullOrWhiteSpace(request.City))
            addressParts.Add(request.City);
        if (!string.IsNullOrWhiteSpace(request.Region))
            addressParts.Add(request.Region);
        if (!string.IsNullOrWhiteSpace(request.PostalCode))
            addressParts.Add(request.PostalCode);
        if (!string.IsNullOrWhiteSpace(request.Country))
            addressParts.Add(request.Country);

        var addressValue = addressParts.Count > 0
            ? string.Join(", ", addressParts)
            : location.Address.Value;
        var address = Address.Create(addressValue);

        location = location.Update(name, timezone, address, now);

        _context.Locations.Update(location);
        await _context.SaveChangesAsync(cancellationToken);

        return location;
    }
}
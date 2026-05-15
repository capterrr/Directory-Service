using Domain.Entities;
using Domain.ValueObjects;
using DirectoryService.Application.Commands.Location;
using Infrastructure.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Handlers.Location;

public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, Domain.Entities.Location>
{
    private readonly DirectoryDbContext _context;

    public CreateLocationHandler(DirectoryDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Location> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var name = Name.Create(request.Name);

        var exists = await _context.Locations
            .AnyAsync(l => l.Name.Value == name.Value, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"Location with name '{name.Value}' already exists.");
        }

        var now = UtcDateTime.Create(DateTime.UtcNow);
        var id = EntityId.Create();

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
            : string.Empty;

        var location = Domain.Entities.Location.Create(
            id,
            Address.Create(addressValue),
            name,
            Timezone.Create(request.Timezone ?? "UTC"),
            now,
            now);

        _context.Locations.Add(location);

        await _context.SaveChangesAsync(cancellationToken);

        return location;
    }
}
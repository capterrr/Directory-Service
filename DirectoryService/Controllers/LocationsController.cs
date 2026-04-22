using DirectoryService.Storage;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    /// <summary>
    /// Get all active locations
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var locations = LocationStorage.GetAll();
            return Ok(locations.Select(l => new
            {
                l.Id,
                l.Name,
                l.Address,
                l.Timezone,
                l.IsActive,
                l.CreatedAt,
                l.UpdatedAt
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get location by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var location = LocationStorage.GetById(EntityId.Create(id));
            if (location == null)
            {
                return NotFound(new { message = "Location not found" });
            }

            return Ok(new
            {
                location.Id,
                location.Name,
                location.Address,
                location.Timezone,
                location.IsActive,
                location.CreatedAt,
                location.UpdatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Create a new location
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateLocationRequest request)
    {
        try
        {
            var locationId = EntityId.Create(Guid.NewGuid());
            var addressString = $"{request.Street}, {request.City}, {request.State} {request.ZipCode}";
            var address = Address.Create(addressString);
            var name = Name.Create(request.Name);
            var timezone = Timezone.Create(request.Timezone);
            var now = UtcDateTime.Create(DateTime.UtcNow);

            var location = Location.Create(
                locationId,
                address,
                name,
                timezone,
                now,
                now);

            LocationStorage.Add(location);

            return CreatedAtAction(nameof(GetById), new { id = location.Id.Value }, new
            {
                location.Id,
                location.Name,
                location.Address,
                location.Timezone,
                location.IsActive,
                location.CreatedAt,
                location.UpdatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("already exists"))
            {
                return Conflict(new { message = ex.Message });
            }
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update an existing location
    /// </summary>
    [HttpPatch("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateLocationRequest request)
    {
        try
        {
            var locationId = EntityId.Create(id);
            var location = LocationStorage.GetById(locationId);

            if (location == null)
            {
                return NotFound(new { message = "Location not found" });
            }

            var name = Name.Create(request.Name);
            var timezone = Timezone.Create(request.Timezone);
            var addressString = $"{request.Street}, {request.City}, {request.State} {request.ZipCode}";
            var address = Address.Create(addressString);
            var updatedAt = UtcDateTime.Create(DateTime.UtcNow);

            var updatedLocation = location.Update(name, timezone, address, updatedAt);
            LocationStorage.UpdateLocation(updatedLocation);

            return Ok(new
            {
                updatedLocation.Id,
                updatedLocation.Name,
                updatedLocation.Address,
                updatedLocation.Timezone,
                updatedLocation.IsActive,
                updatedLocation.CreatedAt,
                updatedLocation.UpdatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("archived"))
            {
                return NotFound(new { message = "Location not found" });
            }
            if (ex.Message.Contains("already exists"))
            {
                return Conflict(new { message = ex.Message });
            }
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Soft delete (archive) a location
    /// </summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var locationId = EntityId.Create(id);
            LocationStorage.Remove(locationId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("not found") || ex.Message.Contains("archived"))
            {
                return NotFound(new { message = "Location not found" });
            }
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Hard delete (permanent removal) a location
    /// </summary>
    [HttpDelete("{id:guid}/permanent")]
    public IActionResult HardDelete(Guid id)
    {
        try
        {
            var locationId = EntityId.Create(id);
            LocationStorage.HardRemove(locationId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = "Location not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }
}

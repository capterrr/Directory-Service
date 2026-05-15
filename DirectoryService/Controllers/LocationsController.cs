using DirectoryService.Application.Commands.Location;
using DirectoryService.Application.Queries.Location;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetAllLocationsQuery();
            var locations = await _mediator.Send(query, cancellationToken);
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetLocationByIdQuery { Id = id };
            var location = await _mediator.Send(query, cancellationToken);

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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var location = await _mediator.Send(request, cancellationToken);

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
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateLocationCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Region = request.Region,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Timezone = request.Timezone
            };

            var location = await _mediator.Send(command, cancellationToken);

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
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteLocationCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
            {
                return NotFound(new { message = "Location not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }
}

using DirectoryService.Storage;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    /// <summary>
    /// Get all active positions
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var positions = PositionStorage.GetAll();
            return Ok(positions.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.IsActive,
                p.CreatedAt,
                p.UpdatedAt
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get position by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var position = PositionStorage.GetById(EntityId.Create(id));
            if (position == null)
            {
                return NotFound(new { message = "Position not found" });
            }

            return Ok(new
            {
                position.Id,
                position.Name,
                position.Description,
                position.IsActive,
                position.CreatedAt,
                position.UpdatedAt
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
    /// Create a new position
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreatePositionRequest request)
    {
        try
        {
            var positionId = EntityId.Create(Guid.NewGuid());
            var name = Name.Create(request.Name);
            var description = Description.Create(request.Description);
            var now = UtcDateTime.Create(DateTime.UtcNow);

            var position = Position.Create(
                positionId,
                name,
                description,
                IsActive.Create(true),
                now,
                now);

            PositionStorage.Add(position);

            return CreatedAtAction(nameof(GetById), new { id = position.Id.Value }, new
            {
                position.Id,
                position.Name,
                position.Description,
                position.IsActive,
                position.CreatedAt,
                position.UpdatedAt
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
    /// Update an existing position
    /// </summary>
    [HttpPatch("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdatePositionRequest request)
    {
        try
        {
            var positionId = EntityId.Create(id);
            var position = PositionStorage.GetById(positionId);

            if (position == null)
            {
                return NotFound(new { message = "Position not found" });
            }

            var updatedAt = UtcDateTime.Create(DateTime.UtcNow);
            var updatedPosition = position;

            if (!string.IsNullOrEmpty(request.Name))
            {
                var name = Name.Create(request.Name);
                updatedPosition = updatedPosition.UpdateName(name, updatedAt);
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                var description = Description.Create(request.Description);
                updatedPosition = updatedPosition.UpdateDescription(description, updatedAt);
            }

            PositionStorage.UpdatePosition(updatedPosition);

            return Ok(new
            {
                updatedPosition.Id,
                updatedPosition.Name,
                updatedPosition.Description,
                updatedPosition.IsActive,
                updatedPosition.CreatedAt,
                updatedPosition.UpdatedAt
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
                return NotFound(new { message = "Position not found" });
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
    /// Soft delete (archive) a position
    /// </summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var positionId = EntityId.Create(id);
            PositionStorage.Remove(positionId);
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
                return NotFound(new { message = "Position not found" });
            }
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Hard delete (permanent removal) a position
    /// </summary>
    [HttpDelete("{id:guid}/permanent")]
    public IActionResult HardDelete(Guid id)
    {
        try
        {
            var positionId = EntityId.Create(id);
            PositionStorage.HardRemove(positionId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = "Position not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }
}

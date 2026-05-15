using DirectoryService.Application.Commands.Position;
using DirectoryService.Application.Queries.Position;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PositionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all positions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetAllPositionsQuery();
            var positions = await _mediator.Send(query, cancellationToken);
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
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetPositionByIdQuery { Id = id };
            var position = await _mediator.Send(query, cancellationToken);

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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Create a new position
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var position = await _mediator.Send(request, cancellationToken);

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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update an existing position
    /// </summary>
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdatePositionCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description
            };

            var position = await _mediator.Send(command, cancellationToken);

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

    /// <summary>
    /// Delete a position
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeletePositionCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
            {
                return NotFound(new { message = "Position not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
        }
    }
}

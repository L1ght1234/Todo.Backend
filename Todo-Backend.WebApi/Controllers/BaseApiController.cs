using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Todo_Backend.Controllers;

[Route("api/[controller]/[action]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected ActionResult<T> HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (EqualityComparer<T>.Default.Equals(result.Value, default))
                return NotFound();

            return Ok(result.Value);
        }

        return BadRequest(new
        {
            Errors = result.Errors.Select(e => e.Message)
        });
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(new
        {
            Errors = result.Errors.Select(e => e.Message)
        });
    }
}
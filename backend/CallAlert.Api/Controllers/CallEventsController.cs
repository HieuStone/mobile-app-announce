using System.Security.Claims;
using CallAlert.Api.Dtos.CallEvents;
using CallAlert.Api.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CallAlert.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CallEventsController : ControllerBase
{
    private readonly ICallEventService _service;

    public CallEventsController(ICallEventService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] bool? isWatched)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _service.GetAsync(userId, from, to, isWatched);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCallEventRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _service.CreateAsync(userId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private bool TryGetUserId(out int userId)
    {
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId > 0)
        {
            return true;
        }

        userId = 0;
        return false;
    }
}



using System.Security.Claims;
using CallAlert.Api.Dtos.WatchNumbers;
using CallAlert.Api.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CallAlert.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WatchNumbersController : ControllerBase
{
    private readonly IWatchNumberService _service;

    public WatchNumbersController(IWatchNumberService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _service.GetAsync(userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWatchNumberRequest request)
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateWatchNumberRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _service.UpdateAsync(userId, id, request);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var deleted = await _service.DeleteAsync(userId, id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
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



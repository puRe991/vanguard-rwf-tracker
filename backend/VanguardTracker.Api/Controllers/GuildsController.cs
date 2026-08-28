using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.DTOs;

namespace VanguardTracker.Api.Controllers;

[ApiController]
[Route("api/guilds")]
public class GuildsController(VanguardDbContext db) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GuildDto>> GetById(Guid id, CancellationToken ct)
    {
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (guild is null) return NotFound();

        return Ok(new GuildDto(guild.Id, guild.Name, guild.Region, guild.FoundedYear));
    }
}

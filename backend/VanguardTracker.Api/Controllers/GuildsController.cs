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

    /// <summary>
    /// Recap + komplette Kill-Historie einer Gilde über alle Seasons hinweg, inkl.
    /// Beleg-/Video-Link je Kill (Warcraft-Logs-Fight-Link für live getrackte Kills,
    /// Method-Quellenlink für kuratierte Historie).
    /// </summary>
    [HttpGet("{id:guid}/profile")]
    public async Task<ActionResult<GuildProfileDto>> GetProfile(Guid id, CancellationToken ct)
    {
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (guild is null) return NotFound();

        var kills = await db.Kills
            .Where(k => k.GuildId == id)
            .Include(k => k.Boss).ThenInclude(b => b!.Raid).ThenInclude(r => r!.Season).ThenInclude(s => s!.Expansion)
            .OrderByDescending(k => k.Timestamp)
            .ToListAsync(ct);

        var history = kills
            .Select(k => new GuildHistoryKillDto(
                k.Boss?.Raid?.Season?.Expansion?.Name ?? "Unbekannt",
                k.Boss?.Raid?.Name ?? "Unbekannt",
                k.Boss?.Name ?? "Unbekannt",
                k.Timestamp,
                k.PullCount,
                k.SourceUrl
            ))
            .ToList();

        return Ok(new GuildProfileDto(
            new GuildDto(guild.Id, guild.Name, guild.Region, guild.FoundedYear),
            guild.Status.ToString().ToLowerInvariant(),
            guild.DisbandedYear,
            guild.Bio,
            new GuildLinksDto(guild.TwitchUrl, guild.YoutubeUrl, guild.TwitterUrl, guild.WebsiteUrl),
            history
        ));
    }
}

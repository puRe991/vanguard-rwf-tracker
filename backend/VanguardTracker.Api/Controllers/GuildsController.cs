using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.DTOs;

namespace VanguardTracker.Api.Controllers;

[ApiController]
[Route("api/guilds")]
public class GuildsController(VanguardDbContext db) : ControllerBase
{
    /// <summary>
    /// Fortschritt einer Gilde in der aktuellen Race (gleiche Form wie ein einzelner
    /// Eintrag aus GET /api/races/current). Liefert 404, wenn es keine aktuelle Race
    /// gibt oder die Gilde darin keinen einzigen Kill hat — z. B. für längst
    /// aufgelöste Gilden, die nur historische Kills haben. Das Frontend blendet den
    /// "Aktueller Fortschritt"-Block dann einfach aus, statt auf ein leeres Objekt zu treffen.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GuildRaceEntryDto>> GetById(Guid id, CancellationToken ct)
    {
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (guild is null) return NotFound();

        var raid = await db.Raids
            .Include(r => r.Bosses)
            .Where(r => r.MythicOpenAt != null && r.MythicOpenAt <= DateTimeOffset.UtcNow)
            .OrderByDescending(r => r.MythicOpenAt)
            .FirstOrDefaultAsync(ct);

        if (raid is null) return NotFound();

        var bossIds = raid.Bosses.Select(b => b.Id).ToHashSet();
        var kills = await db.Kills
            .Where(k => k.GuildId == id && bossIds.Contains(k.BossId))
            .ToListAsync(ct);

        if (kills.Count == 0) return NotFound();

        var bosses = raid.Bosses
            .OrderBy(b => b.Order)
            .Select(b =>
            {
                var kill = kills.FirstOrDefault(k => k.BossId == b.Id);
                return new BossProgressDto(
                    b.Id,
                    b.Name,
                    b.Order,
                    kill is not null ? "killed" : "locked",
                    kill?.PullCount,
                    kill?.Timestamp
                );
            })
            .ToList();

        return Ok(new GuildRaceEntryDto(
            new GuildDto(guild.Id, guild.Name, guild.Region, guild.FoundedYear),
            0,
            bosses,
            kills.Count,
            kills.Sum(k => k.PullCount),
            kills.Max(k => (DateTimeOffset?)k.Timestamp)
        ));
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

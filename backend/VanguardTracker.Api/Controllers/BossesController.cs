using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.DTOs;

namespace VanguardTracker.Api.Controllers;

[ApiController]
[Route("api/bosses")]
public class BossesController(VanguardDbContext db) : ControllerBase
{
    /// <summary>
    /// Pull-Verlauf je Gilde für einen Boss, für den Recharts-Vergleich auf der
    /// Boss-Detailseite. Jeder Kill liefert die kumulierte Pull-Anzahl bis zum Kill.
    /// </summary>
    [HttpGet("{id:guid}/pulls")]
    public async Task<ActionResult<List<BossPullSeriesDto>>> GetPulls(Guid id, CancellationToken ct)
    {
        var kills = await db.Kills
            .Where(k => k.BossId == id)
            .Include(k => k.Guild)
            .ToListAsync(ct);

        var series = kills
            .GroupBy(k => k.GuildId)
            .Select(g =>
            {
                var guild = g.First().Guild!;
                var points = g
                    .OrderBy(k => k.Timestamp)
                    .SelectMany(k => Enumerable.Range(1, k.PullCount)
                        .Select(n => new PullSeriesPointDto(n, k.Timestamp)))
                    .ToList();

                return new BossPullSeriesDto(
                    new GuildDto(guild.Id, guild.Name, guild.Region, guild.FoundedYear),
                    points,
                    true
                );
            })
            .ToList();

        return Ok(series);
    }
}

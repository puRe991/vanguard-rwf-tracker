using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.DTOs;

namespace VanguardTracker.Api.Controllers;

[ApiController]
[Route("api/history")]
public class HistoryController(VanguardDbContext db) : ControllerBase
{
    /// <summary>
    /// Zeitleiste: pro Raid-Tier die World-First-Gilde (frühester finaler Boss-Kill)
    /// und deren Pull-Anzahl. Filterbar nach Expansion/Season.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<HistoryTierDto>>> Get(
        [FromQuery] string? expansion,
        [FromQuery] int? season,
        CancellationToken ct)
    {
        var query = db.Raids
            .Include(r => r.Season).ThenInclude(s => s!.Expansion)
            .Include(r => r.Bosses).ThenInclude(b => b.Kills).ThenInclude(k => k.Guild)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(expansion))
            query = query.Where(r => r.Season!.Expansion!.Name == expansion);

        if (season.HasValue)
            query = query.Where(r => r.Season!.Number == season.Value);

        var raids = await query.ToListAsync(ct);

        var tiers = raids
            .Select(raid =>
            {
                var finalBoss = raid.Bosses.OrderByDescending(b => b.Order).FirstOrDefault();
                var worldFirstKill = finalBoss?.Kills
                    .OrderBy(k => k.Timestamp)
                    .FirstOrDefault();

                return new HistoryTierDto(
                    raid.Season?.Expansion?.Name ?? "Unbekannt",
                    raid.Season?.Number ?? 0,
                    raid.Name,
                    worldFirstKill?.Guild?.Name ?? "—",
                    worldFirstKill?.PullCount ?? 0,
                    worldFirstKill is not null
                        ? DateOnly.FromDateTime(worldFirstKill.Timestamp.UtcDateTime)
                        : default
                );
            })
            .ToList();

        return Ok(tiers);
    }
}

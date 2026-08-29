using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.DTOs;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Controllers;

[ApiController]
[Route("api/races")]
public class RacesController(VanguardDbContext db) : ControllerBase
{
    /// <summary>
    /// Aktuelle Race: das jüngste Raid-Tier mit offenem Mythic-Modus, inkl. Boss-Rail
    /// je Gilde, sortiert nach Anzahl getöteter Bosse und letztem Kill-Zeitpunkt.
    /// </summary>
    [HttpGet("current")]
    public async Task<ActionResult<List<GuildRaceEntryDto>>> GetCurrent(CancellationToken ct)
    {
        var raid = await db.Raids
            .Include(r => r.Bosses)
            .Where(r => r.MythicOpenAt != null && r.MythicOpenAt <= DateTimeOffset.UtcNow)
            .OrderByDescending(r => r.MythicOpenAt)
            .FirstOrDefaultAsync(ct);

        if (raid is null)
        {
            return Ok(new List<GuildRaceEntryDto>());
        }

        var bossIds = raid.Bosses.Select(b => b.Id).ToList();

        // Nur moderierte Kills zählen in die Live-Wertung — Community-Einreichungen
        // (KillsController.Submit) liegen als Unconfirmed vor, bis ein Moderator sie
        // freigibt, und dürfen die Rangliste bis dahin nicht verschieben.
        var killsByBoss = await db.Kills
            .Where(k => bossIds.Contains(k.BossId) && k.Status == KillStatus.Confirmed)
            .Include(k => k.Guild)
            .ToListAsync(ct);

        var entries = killsByBoss
            .GroupBy(k => k.GuildId)
            .Select(g =>
            {
                var guild = g.First().Guild!;
                var killedBossIds = g.Select(k => k.BossId).ToHashSet();
                var bosses = raid.Bosses
                    .OrderBy(b => b.Order)
                    .Select(b =>
                    {
                        var kill = g.FirstOrDefault(k => k.BossId == b.Id);
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

                return new GuildRaceEntryDto(
                    new GuildDto(guild.Id, guild.Name, guild.Region, guild.FoundedYear),
                    0,
                    bosses,
                    killedBossIds.Count,
                    g.Sum(k => k.PullCount),
                    g.Max(k => (DateTimeOffset?)k.Timestamp)
                );
            })
            .OrderByDescending(e => e.BossesKilled)
            .ThenBy(e => e.LastKillAt)
            .Select((e, i) => e with { Rank = i + 1 })
            .ToList();

        return Ok(entries);
    }

    /// <summary>
    /// Jüngste Kills der aktuellen Race als Ticker-Feed (initialer Fetch beim
    /// Seitenaufruf; danach übernimmt SignalR via RaceHub/TickerEvent das Live-Update).
    /// Pull-Milestones werden nicht persistiert und tauchen daher nur live auf, nicht
    /// rückwirkend in diesem Feed.
    /// </summary>
    [HttpGet("current/ticker")]
    public async Task<ActionResult<List<LiveTickerEventDto>>> GetCurrentTicker(CancellationToken ct)
    {
        var raid = await db.Raids
            .Include(r => r.Bosses)
            .Where(r => r.MythicOpenAt != null && r.MythicOpenAt <= DateTimeOffset.UtcNow)
            .OrderByDescending(r => r.MythicOpenAt)
            .FirstOrDefaultAsync(ct);

        if (raid is null)
        {
            return Ok(new List<LiveTickerEventDto>());
        }

        var bossIds = raid.Bosses.Select(b => b.Id).ToList();

        var events = await db.Kills
            .Where(k => bossIds.Contains(k.BossId) && k.Status == KillStatus.Confirmed)
            .Include(k => k.Guild)
            .Include(k => k.Boss)
            .OrderByDescending(k => k.Timestamp)
            .Take(20)
            .Select(k => new LiveTickerEventDto(
                k.Id,
                k.Guild!.Name,
                k.Boss!.Name,
                k.Guild!.Name + " besiegt " + k.Boss!.Name,
                k.Timestamp,
                "kill"
            ))
            .ToListAsync(ct);

        return Ok(events);
    }
}

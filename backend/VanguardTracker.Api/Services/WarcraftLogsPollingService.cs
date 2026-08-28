using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.DTOs;
using VanguardTracker.Api.Hubs;
using VanguardTracker.Api.Models;
using VanguardTracker.Api.WarcraftLogs;

namespace VanguardTracker.Api.Services;

/// <summary>
/// Pollt periodisch die Warcraft Logs API für die aktuelle Mythic-Race, erkennt neue
/// Pulls/Kills anhand der Fight-Logs jeder getrackten Gilde, persistiert bestätigte
/// Kills und pusht Live-Updates über den RaceHub.
///
/// Getrackt wird nur, was gemappt ist: Gilden brauchen WarcraftLogs*-Felder gesetzt,
/// Bosse eine WarcraftLogsEncounterId. Ungemapptes wird stillschweigend übersprungen,
/// damit Community-kuratierte (vor-Cataclysm) Inhalte nicht versehentlich angefasst werden.
/// </summary>
public class WarcraftLogsPollingService(
    IServiceScopeFactory scopeFactory,
    IHubContext<RaceHub> hubContext,
    IOptions<WarcraftLogsOptions> options,
    ILogger<WarcraftLogsPollingService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(Math.Max(1, options.Value.PollIntervalMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollOnceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Polling der Warcraft Logs API");
            }

            await Task.Delay(interval, stoppingToken);
        }
    }

    private async Task PollOnceAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(options.Value.ClientId) || string.IsNullOrWhiteSpace(options.Value.ClientSecret))
        {
            logger.LogDebug("Warcraft-Logs-Zugangsdaten fehlen (WarcraftLogs:ClientId/ClientSecret) — Polling übersprungen.");
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VanguardDbContext>();
        var wcl = scope.ServiceProvider.GetRequiredService<WarcraftLogsClient>();

        var raid = await db.Raids
            .Include(r => r.Bosses)
            .Where(r => r.MythicOpenAt != null && r.MythicOpenAt <= DateTimeOffset.UtcNow)
            .OrderByDescending(r => r.MythicOpenAt)
            .FirstOrDefaultAsync(ct);

        if (raid is null) return;

        var trackedBosses = raid.Bosses.Where(b => b.WarcraftLogsEncounterId.HasValue).ToList();
        if (trackedBosses.Count == 0) return;

        var trackedGuilds = await db.Guilds
            .Where(g => g.WarcraftLogsGuildName != null
                        && g.WarcraftLogsServerSlug != null
                        && g.WarcraftLogsServerRegion != null)
            .ToListAsync(ct);

        if (trackedGuilds.Count == 0) return;

        var graceCutoff = DateTimeOffset.UtcNow.AddMinutes(-options.Value.ReportFinalizationGraceMinutes);

        foreach (var guild in trackedGuilds)
        {
            foreach (var boss in trackedBosses)
            {
                await PollGuildBossAsync(db, wcl, guild, boss, raid.MythicOpenAt!.Value, graceCutoff, ct);
            }
        }
    }

    private async Task PollGuildBossAsync(
        VanguardDbContext db,
        WarcraftLogsClient wcl,
        Guild guild,
        Boss boss,
        DateTimeOffset raidOpensAt,
        DateTimeOffset graceCutoff,
        CancellationToken ct)
    {
        var progress = await db.EncounterProgresses
            .FirstOrDefaultAsync(p => p.GuildId == guild.Id && p.BossId == boss.Id, ct);

        if (progress is null)
        {
            progress = new EncounterProgress
            {
                Id = Guid.NewGuid(),
                GuildId = guild.Id,
                BossId = boss.Id,
                UpdatedAt = raidOpensAt,
            };
            db.EncounterProgresses.Add(progress);
        }

        if (progress.Killed) return;

        var oldPullCount = progress.PullCount;
        var processedCodes = progress.ProcessedReportCodesCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();

        List<WclReport> reports;
        try
        {
            reports = await wcl.GetGuildReportsAsync(
                guild.WarcraftLogsGuildName!,
                guild.WarcraftLogsServerSlug!,
                guild.WarcraftLogsServerRegion!,
                raidOpensAt,
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Reports für Gilde {Guild} konnten nicht geladen werden", guild.Name);
            return;
        }

        var newReports = reports
            .Where(r => !processedCodes.Contains(r.Code))
            .Where(r => DateTimeOffset.FromUnixTimeMilliseconds((long)r.EndTime) <= graceCutoff)
            .OrderBy(r => r.StartTime)
            .ToList();

        foreach (var report in newReports)
        {
            WclReportFights? fights;
            try
            {
                fights = await wcl.GetReportFightsAsync(report.Code, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Fights für Report {Report} konnten nicht geladen werden", report.Code);
                continue;
            }

            if (fights is null)
            {
                processedCodes.Add(report.Code);
                continue;
            }

            var relevantFights = fights.Fights
                .Where(f => f.EncounterId == boss.WarcraftLogsEncounterId)
                .OrderBy(f => f.StartTime)
                .ToList();

            foreach (var fight in relevantFights)
            {
                progress.PullCount++;

                if (fight.Kill != true) continue;

                var killTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(
                    (long)(fights.StartTime + fight.EndTime));

                List<string>? roster = null;
                try
                {
                    roster = await wcl.GetFightRosterAsync(report.Code, fight.Id, ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Roster für Report {Report} Fight {Fight} konnte nicht geladen werden", report.Code, fight.Id);
                }

                db.Kills.Add(new Kill
                {
                    Id = Guid.NewGuid(),
                    BossId = boss.Id,
                    GuildId = guild.Id,
                    Timestamp = killTimestamp,
                    PullCount = progress.PullCount,
                    SourceUrl = $"https://www.warcraftlogs.com/reports/{report.Code}#fight={fight.Id}",
                    Status = KillStatus.Confirmed,
                    Roster = roster,
                });

                progress.Killed = true;
                progress.KilledAt = killTimestamp;

                await BroadcastTickerEventAsync(
                    guild.Name, boss.Name,
                    $"{guild.Name} besiegt {boss.Name} — Pull #{progress.PullCount}",
                    killTimestamp, "kill", ct);

                break;
            }

            processedCodes.Add(report.Code);
            if (progress.Killed) break;
        }

        progress.ProcessedReportCodesCsv = string.Join(',', processedCodes);
        progress.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        if (progress.PullCount != oldPullCount)
        {
            if (!progress.Killed)
            {
                await BroadcastTickerEventAsync(
                    guild.Name, boss.Name,
                    $"{guild.Name} bei Pull #{progress.PullCount} auf {boss.Name}",
                    DateTimeOffset.UtcNow, "pull-milestone", ct);
            }

            await hubContext.Clients.All.SendAsync("RaceUpdated", ct);
        }
    }

    private Task BroadcastTickerEventAsync(
        string guildName,
        string bossName,
        string message,
        DateTimeOffset timestamp,
        string kind,
        CancellationToken ct)
    {
        var dto = new LiveTickerEventDto(Guid.NewGuid(), guildName, bossName, message, timestamp, kind);
        return hubContext.Clients.All.SendAsync("TickerEvent", dto, ct);
    }
}

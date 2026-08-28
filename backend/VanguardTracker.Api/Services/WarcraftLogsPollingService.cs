using Microsoft.AspNetCore.SignalR;
using VanguardTracker.Api.Hubs;

namespace VanguardTracker.Api.Services;

/// <summary>
/// Phase 2: pollt periodisch die Warcraft Logs API für die aktuelle Race,
/// schreibt neue Kills/Pulls in die Datenbank und pusht sie über RaceHub.
/// Aktuell ein Gerüst ohne echte API-Anbindung.
/// </summary>
public class WarcraftLogsPollingService(
    IHubContext<RaceHub> hubContext,
    ILogger<WarcraftLogsPollingService> logger
) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // TODO Phase 2: Warcraft Logs GraphQL-API abfragen, neue Kills/Pulls
                // persistieren und via hubContext.Clients.All.SendAsync("TickerEvent", ...) pushen.
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Polling der Warcraft Logs API");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}

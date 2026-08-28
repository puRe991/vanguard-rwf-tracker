using Microsoft.AspNetCore.SignalR;

namespace VanguardTracker.Api.Hubs;

/// <summary>
/// Pusht neue Kills und Pull-Updates an alle verbundenen Clients.
/// Server-seitig ausgelöst vom Polling-Hintergrundjob (Services/WarcraftLogsPollingService),
/// nicht von Client-Methoden.
/// </summary>
public class RaceHub : Hub;

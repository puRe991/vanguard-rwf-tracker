namespace VanguardTracker.Api.Models;

public class Guild
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Region { get; set; }
    public int? FoundedYear { get; set; }

    /// <summary>
    /// Warcraft-Logs-Gilden-Identifikation (Name + Server + Region, wie von der WCL-API
    /// erwartet). Null, solange die Gilde nicht für automatisches Live-Tracking
    /// eingerichtet ist — der Poller überspringt sie dann.
    /// </summary>
    public string? WarcraftLogsGuildName { get; set; }
    public string? WarcraftLogsServerSlug { get; set; }
    public string? WarcraftLogsServerRegion { get; set; }

    public List<Kill> Kills { get; set; } = [];
}

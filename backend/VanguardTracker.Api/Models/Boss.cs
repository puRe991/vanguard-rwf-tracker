namespace VanguardTracker.Api.Models;

public class Boss
{
    public Guid Id { get; set; }
    public Guid RaidId { get; set; }
    public Raid? Raid { get; set; }

    public required string Name { get; set; }
    public int Order { get; set; }

    /// <summary>Warcraft-Logs-"encounterID" für diesen Boss. Null, solange nicht gemappt —
    /// der Poller überspringt Bosse ohne Mapping.</summary>
    public int? WarcraftLogsEncounterId { get; set; }

    public List<Kill> Kills { get; set; } = [];
}

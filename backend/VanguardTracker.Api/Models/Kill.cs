namespace VanguardTracker.Api.Models;

public enum KillStatus
{
    Unconfirmed = 0,
    Confirmed = 1,
}

public class Kill
{
    public Guid Id { get; set; }

    public Guid BossId { get; set; }
    public Boss? Boss { get; set; }

    public Guid GuildId { get; set; }
    public Guild? Guild { get; set; }

    public DateTimeOffset Timestamp { get; set; }
    public int PullCount { get; set; }
    public string? SourceUrl { get; set; }
    public KillStatus Status { get; set; } = KillStatus.Unconfirmed;

    /// <summary>
    /// Spielernamen des Kill-Rosters — nur gesetzt, wenn tatsächlich aus einer
    /// Quelle (z. B. Warcraft-Logs-Report) bekannt. Nie für kuratierte
    /// Historien-Kills ohne Rosterdaten befüllen.
    /// </summary>
    public List<string>? Roster { get; set; }
}

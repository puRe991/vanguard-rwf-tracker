namespace VanguardTracker.Api.Models;

/// <summary>
/// Poller-interner Fortschritt je Gilde/Boss — getrennt von der öffentlichen
/// <see cref="Kill"/>-Entität, damit der Warcraft-Logs-Poller nachvollziehen kann,
/// welche Reports/Pulls er schon verarbeitet hat, ohne Kills zu duplizieren.
/// </summary>
public class EncounterProgress
{
    public Guid Id { get; set; }

    public Guid GuildId { get; set; }
    public Guild? Guild { get; set; }

    public Guid BossId { get; set; }
    public Boss? Boss { get; set; }

    public int PullCount { get; set; }
    public bool Killed { get; set; }
    public DateTimeOffset? KilledAt { get; set; }

    /// <summary>Kommagetrennte Liste bereits vollständig verarbeiteter WCL-Report-Codes.</summary>
    public string ProcessedReportCodesCsv { get; set; } = "";

    public DateTimeOffset UpdatedAt { get; set; }
}

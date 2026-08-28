namespace VanguardTracker.Api.Models;

public enum GuildStatus
{
    /// <summary>Kein gesicherter Status bekannt — Standard für die meisten importierten Gilden.</summary>
    Unknown = 0,
    Active = 1,
    Disbanded = 2,
    /// <summary>Existiert weiter, tritt aber nicht mehr im Race-to-World-First-Spitzenfeld an
    /// (z. B. Blood Legion nach 2015) — anders als vollständig aufgelöst.</summary>
    Retired = 3,
}

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

    /// <summary>
    /// Recap-Text (Entstehung, Höhepunkte, Auflösung/Nachfolge). Nur für recherchierte,
    /// belegte Gilden gesetzt (siehe GuildProfileSeeder) — bleibt für alle anderen null,
    /// statt Geschichte zu erfinden.
    /// </summary>
    public string? Bio { get; set; }
    public GuildStatus Status { get; set; } = GuildStatus.Unknown;
    public int? DisbandedYear { get; set; }

    public string? TwitchUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    public List<Kill> Kills { get; set; } = [];
}

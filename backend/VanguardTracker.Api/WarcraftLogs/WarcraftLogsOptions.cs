namespace VanguardTracker.Api.WarcraftLogs;

public class WarcraftLogsOptions
{
    public const string SectionName = "WarcraftLogs";

    /// <summary>Client-ID/-Secret einer V2-Client-App unter warcraftlogs.com/api/clients.</summary>
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";

    public string OAuthTokenUrl { get; set; } = "https://www.warcraftlogs.com/oauth/token";
    public string ApiUrl { get; set; } = "https://www.warcraftlogs.com/api/v2/client";

    /// <summary>WCL-Difficulty-ID für Mythic-Raids (aktuell 5 für Retail-Content).</summary>
    public int MythicDifficultyId { get; set; } = 5;

    public int PollIntervalMinutes { get; set; } = 2;

    /// <summary>
    /// Ein Report wird erst verarbeitet, wenn sein letzter Fight länger als diese
    /// Zeitspanne zurückliegt — verhindert, dass ein noch live hochgeladener Report
    /// (unvollständige Pull-Liste) vorzeitig als final gilt.
    /// </summary>
    public int ReportFinalizationGraceMinutes { get; set; } = 10;
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace VanguardTracker.Api.WarcraftLogs;

public record WclGraphQlResponse<T>(
    [property: JsonPropertyName("data")] T? Data,
    [property: JsonPropertyName("errors")] List<WclError>? Errors
);

public record WclError([property: JsonPropertyName("message")] string Message);

public record WclGuildReportsData(
    [property: JsonPropertyName("guildData")] WclGuildData? GuildData
);

public record WclGuildData([property: JsonPropertyName("guild")] WclGuild? Guild);

public record WclGuild([property: JsonPropertyName("reports")] WclReportConnection? Reports);

public record WclReportConnection([property: JsonPropertyName("data")] List<WclReport> Data);

public record WclReport(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("startTime")] double StartTime,
    [property: JsonPropertyName("endTime")] double EndTime
);

public record WclReportFightsData(
    [property: JsonPropertyName("reportData")] WclReportDataWrapper? ReportData
);

public record WclReportDataWrapper([property: JsonPropertyName("report")] WclReportFights? Report);

public record WclReportFights(
    [property: JsonPropertyName("startTime")] double StartTime,
    [property: JsonPropertyName("fights")] List<WclFight> Fights
);

public record WclFight(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("encounterID")] int EncounterId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("kill")] bool? Kill,
    [property: JsonPropertyName("startTime")] double StartTime,
    [property: JsonPropertyName("endTime")] double EndTime,
    [property: JsonPropertyName("difficulty")] int? Difficulty
);

public record WclReportPlayerDetailsData(
    [property: JsonPropertyName("reportData")] WclReportPlayerDetailsWrapper? ReportData
);

public record WclReportPlayerDetailsWrapper(
    [property: JsonPropertyName("report")] WclReportPlayerDetails? Report
);

// "playerDetails" ist in der WCL-v2-API ein rohes JSON-Scalar-Feld (kein
// typisiertes GraphQL-Objekt), daher hier als JsonElement roh eingelesen.
public record WclReportPlayerDetails(
    [property: JsonPropertyName("playerDetails")] JsonElement PlayerDetails
);

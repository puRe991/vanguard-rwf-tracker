using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace VanguardTracker.Api.WarcraftLogs;

/// <summary>
/// Dünner GraphQL-Client für die Warcraft-Logs-API v2 (client-Endpoint).
/// Kennt nur die zwei Abfragen, die der Polling-Job braucht: Reports einer Gilde
/// seit einem Zeitpunkt, und die Fights (Pulls) eines einzelnen Reports.
/// </summary>
public class WarcraftLogsClient(
    HttpClient httpClient,
    WarcraftLogsAuthClient authClient,
    IOptions<WarcraftLogsOptions> options)
{
    private const string GuildReportsQuery = """
        query GuildReports($guildName: String!, $serverSlug: String!, $serverRegion: String!, $startTime: Float) {
          guildData {
            guild(name: $guildName, serverSlug: $serverSlug, serverRegion: $serverRegion) {
              reports(startTime: $startTime) {
                data { code startTime endTime }
              }
            }
          }
        }
        """;

    private const string ReportFightsQuery = """
        query ReportFights($code: String!, $difficulty: Int) {
          reportData {
            report(code: $code) {
              startTime
              fights(difficulty: $difficulty) {
                id
                encounterID
                name
                kill
                startTime
                endTime
                difficulty
              }
            }
          }
        }
        """;

    public async Task<List<WclReport>> GetGuildReportsAsync(
        string guildName,
        string serverSlug,
        string serverRegion,
        DateTimeOffset? since,
        CancellationToken ct)
    {
        var variables = new Dictionary<string, object?>
        {
            ["guildName"] = guildName,
            ["serverSlug"] = serverSlug,
            ["serverRegion"] = serverRegion,
            ["startTime"] = since is null ? null : (double)since.Value.ToUnixTimeMilliseconds(),
        };

        var result = await ExecuteAsync<WclGuildReportsData>(GuildReportsQuery, variables, ct);
        return result?.GuildData?.Guild?.Reports?.Data ?? [];
    }

    public async Task<WclReportFights?> GetReportFightsAsync(string reportCode, CancellationToken ct)
    {
        var variables = new Dictionary<string, object?>
        {
            ["code"] = reportCode,
            ["difficulty"] = options.Value.MythicDifficultyId,
        };

        var result = await ExecuteAsync<WclReportFightsData>(ReportFightsQuery, variables, ct);
        return result?.ReportData?.Report;
    }

    private const string PlayerDetailsQuery = """
        query FightPlayerDetails($code: String!, $fightIDs: [Int]) {
          reportData {
            report(code: $code) {
              playerDetails(fightIDs: $fightIDs)
            }
          }
        }
        """;

    /// <summary>
    /// Liest die Spielernamen (Tanks/Healers/DPS) eines einzelnen Fights aus dem
    /// "playerDetails"-Feld der WCL-API. Gibt null zurück, wenn das Report keine
    /// (mehr) auswertbaren Details für diesen Fight liefert.
    /// </summary>
    public async Task<List<string>?> GetFightRosterAsync(string reportCode, int fightId, CancellationToken ct)
    {
        var variables = new Dictionary<string, object?>
        {
            ["code"] = reportCode,
            ["fightIDs"] = new[] { fightId },
        };

        var result = await ExecuteAsync<WclReportPlayerDetailsData>(PlayerDetailsQuery, variables, ct);
        var root = result?.ReportData?.Report?.PlayerDetails;
        if (root is not { ValueKind: JsonValueKind.Object } detailsRoot)
        {
            return null;
        }

        if (!detailsRoot.TryGetProperty("data", out var data) ||
            !data.TryGetProperty("playerDetails", out var playerDetails))
        {
            return null;
        }

        var names = new List<string>();
        foreach (var group in new[] { "tanks", "healers", "dps" })
        {
            if (!playerDetails.TryGetProperty(group, out var players) || players.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var player in players.EnumerateArray())
            {
                if (player.TryGetProperty("name", out var nameProp) && nameProp.ValueKind == JsonValueKind.String)
                {
                    var name = nameProp.GetString();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        names.Add(name);
                    }
                }
            }
        }

        return names.Count > 0 ? names : null;
    }

    private async Task<T?> ExecuteAsync<T>(string query, Dictionary<string, object?> variables, CancellationToken ct)
    {
        var token = await authClient.GetAccessTokenAsync(ct);

        using var request = new HttpRequestMessage(HttpMethod.Post, options.Value.ApiUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { query, variables });

        using var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<WclGraphQlResponse<T>>(cancellationToken: ct);

        if (payload?.Errors is { Count: > 0 } errors)
        {
            throw new InvalidOperationException(
                $"Warcraft-Logs-API-Fehler: {string.Join("; ", errors.Select(e => e.Message))}");
        }

        return payload is null ? default : payload.Data;
    }
}

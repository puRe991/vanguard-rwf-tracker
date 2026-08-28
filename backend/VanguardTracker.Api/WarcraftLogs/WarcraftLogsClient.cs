using System.Net.Http.Headers;
using System.Net.Http.Json;
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

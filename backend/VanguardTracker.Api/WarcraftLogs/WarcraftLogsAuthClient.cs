using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace VanguardTracker.Api.WarcraftLogs;

/// <summary>
/// Holt und cached ein OAuth2-Client-Credentials-Token für die Warcraft-Logs-API.
/// Als Singleton registriert, damit der Cache über alle Polling-Zyklen hinweg
/// wiederverwendet wird statt bei jedem Request neu zu authentifizieren.
/// </summary>
public class WarcraftLogsAuthClient(HttpClient httpClient, IOptions<WarcraftLogsOptions> options)
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt)
            return _accessToken;

        await _lock.WaitAsync(ct);
        try
        {
            if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt)
                return _accessToken;

            var opts = options.Value;
            using var request = new HttpRequestMessage(HttpMethod.Post, opts.OAuthTokenUrl);
            var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{opts.ClientId}:{opts.ClientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string> { ["grant_type"] = "client_credentials" });

            using var response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<WclTokenResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Leere Antwort vom Warcraft-Logs-Token-Endpoint.");

            _accessToken = payload.AccessToken;
            // 60s Sicherheitsabstand, damit ein Token nicht mitten in einem Request abläuft.
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(60, payload.ExpiresIn - 60));
            return _accessToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private record WclTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn
    );
}

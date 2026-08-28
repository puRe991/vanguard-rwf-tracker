namespace VanguardTracker.Api.DTOs;

public record BossProgressDto(
    Guid Id,
    string Name,
    int Order,
    string Status, // "killed" | "active" | "locked"
    int? PullCount,
    DateTimeOffset? KilledAt
);

public record GuildDto(Guid Id, string Name, string Region, int? FoundedYear);

public record GuildRaceEntryDto(
    GuildDto Guild,
    int Rank,
    List<BossProgressDto> Bosses,
    int BossesKilled,
    int TotalPulls,
    DateTimeOffset? LastKillAt
);

public record LiveTickerEventDto(
    Guid Id,
    string GuildName,
    string BossName,
    string Message,
    DateTimeOffset Timestamp,
    string Kind // "kill" | "pull-milestone" | "live-start"
);

public record HistoryBossDto(string Name, int Order, bool Killed);

public record HistoryTierDto(
    string Expansion,
    int Season,
    string RaidName,
    string WorldFirstGuild,
    int PullCount,
    DateOnly KillDate,
    List<HistoryBossDto> Bosses
);

public record PullSeriesPointDto(int PullNumber, DateTimeOffset Timestamp);

public record BossPullSeriesDto(GuildDto Guild, List<PullSeriesPointDto> Points, bool Killed);

public record GuildLinksDto(string? Twitch, string? YouTube, string? Twitter, string? Website);

public record GuildHistoryKillDto(
    string Expansion,
    string RaidName,
    string BossName,
    DateTimeOffset KillDate,
    int PullCount,
    string? SourceUrl, // Beleg-/Video-Link des Kills
    List<string>? Roster // nur gesetzt, wenn tatsächlich bekannt
);

public record GuildProfileDto(
    GuildDto Guild,
    string Status, // "active" | "disbanded" | "retired" | "unknown"
    int? DisbandedYear,
    string? Bio,
    GuildLinksDto Links,
    List<GuildHistoryKillDto> History // chronologisch absteigend, neuester Kill zuerst
);

public record SubmitKillRequest(
    Guid BossId,
    Guid GuildId,
    DateTimeOffset Timestamp,
    int PullCount,
    string SourceUrl
);

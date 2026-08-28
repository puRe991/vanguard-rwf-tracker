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

public record SubmitKillRequest(
    Guid BossId,
    Guid GuildId,
    DateTimeOffset Timestamp,
    int PullCount,
    string SourceUrl
);

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
}

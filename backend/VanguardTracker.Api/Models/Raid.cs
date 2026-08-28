namespace VanguardTracker.Api.Models;

public class Raid
{
    public Guid Id { get; set; }
    public Guid SeasonId { get; set; }
    public Season? Season { get; set; }

    public required string Name { get; set; }
    public int BossCount { get; set; }

    public DateTimeOffset? NormalOpenAt { get; set; }
    public DateTimeOffset? HeroicOpenAt { get; set; }
    public DateTimeOffset? MythicOpenAt { get; set; }

    public List<Boss> Bosses { get; set; } = [];
}

namespace VanguardTracker.Api.Models;

public class Expansion
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateOnly ReleaseDate { get; set; }

    public List<Season> Seasons { get; set; } = [];
}

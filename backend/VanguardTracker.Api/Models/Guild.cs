namespace VanguardTracker.Api.Models;

public class Guild
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Region { get; set; }
    public int? FoundedYear { get; set; }

    public List<Kill> Kills { get; set; } = [];
}

namespace VanguardTracker.Api.Models;

public class Season
{
    public Guid Id { get; set; }
    public Guid ExpansionId { get; set; }
    public Expansion? Expansion { get; set; }

    public int Number { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public List<Raid> Raids { get; set; } = [];
}

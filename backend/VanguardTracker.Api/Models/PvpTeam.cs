namespace VanguardTracker.Api.Models;

public enum PvpBracket
{
    TwoVTwo = 0,
    ThreeVThree = 1,
    RatedBattleground = 2,
    SoloShuffle = 3,
}

/// <summary>
/// Beta-Feature (Phase 4a): Rating-Leiter-Dashboard, das "Race to World First" fürs
/// PvP-Pendant nachbildet. Kein Blizzard-Battle.net-API-Zugang vorhanden — bis dahin
/// nur über <see cref="Data.PvpDemoSeeder"/> mit klar fiktiven Platzhalterdaten
/// gefüllt, siehe README. Absichtlich kein eigenes Player-Entity/keine Roster-Relation:
/// für die Beta genügt ein einfacher String je Spielername je Team.
/// </summary>
public class PvpTeam
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Region { get; set; }
    public PvpBracket Bracket { get; set; }
    public int Rating { get; set; }
    public List<string> PlayerNames { get; set; } = [];
    public DateTimeOffset UpdatedAt { get; set; }
}

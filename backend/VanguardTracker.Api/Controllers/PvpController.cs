using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.DTOs;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Controllers;

/// <summary>
/// Beta: Rating-Leiter-Dashboard fürs PvP-Pendant zu "Race to World First". Läuft
/// aktuell ausschließlich auf kuratierten Platzhalterdaten (<see cref="Data.PvpDemoSeeder"/>),
/// da keine Blizzard-Battle.net-API-Anbindung existiert — siehe README, Abschnitt
/// "PvP-Rating-Leiter (Beta)".
/// </summary>
[ApiController]
[Route("api/pvp")]
public class PvpController(VanguardDbContext db) : ControllerBase
{
    private static readonly string[] BracketSlugs = ["2v2", "3v3", "rbg", "solo-shuffle"];

    [HttpGet("ladder")]
    public async Task<ActionResult<List<PvpLadderEntryDto>>> GetLadder(
        [FromQuery] string bracket = "3v3",
        CancellationToken ct = default)
    {
        var bracketEnum = ParseBracket(bracket);
        if (bracketEnum is null) return BadRequest($"Unbekannter Bracket-Slug '{bracket}'.");

        var teams = await db.PvpTeams
            .Where(t => t.Bracket == bracketEnum)
            .OrderByDescending(t => t.Rating)
            .ToListAsync(ct);

        var entries = teams
            .Select((t, i) => new PvpLadderEntryDto(
                i + 1,
                t.Id,
                t.Name,
                t.Region,
                ToSlug(t.Bracket),
                t.Rating,
                TierFor(t.Rating),
                t.PlayerNames,
                t.UpdatedAt
            ))
            .ToList();

        return Ok(entries);
    }

    private static PvpBracket? ParseBracket(string slug) => slug switch
    {
        "2v2" => PvpBracket.TwoVTwo,
        "3v3" => PvpBracket.ThreeVThree,
        "rbg" => PvpBracket.RatedBattleground,
        "solo-shuffle" => PvpBracket.SoloShuffle,
        _ => null,
    };

    private static string ToSlug(PvpBracket bracket) => bracket switch
    {
        PvpBracket.TwoVTwo => "2v2",
        PvpBracket.ThreeVThree => "3v3",
        PvpBracket.RatedBattleground => "rbg",
        PvpBracket.SoloShuffle => "solo-shuffle",
        _ => "3v3",
    };

    /// <summary>
    /// Grobe, saisonunabhängige Näherung der offiziellen Rating-Tiers (die echten
    /// Cutoffs legt Blizzard erst am Season-Ende pro Bracket/Region fest). Nur für
    /// die Beta-Darstellung gedacht, keine belastbare Einstufung.
    /// </summary>
    private static string TierFor(int rating) => rating switch
    {
        >= 2400 => "Gladiator",
        >= 2100 => "Elite",
        >= 1800 => "Duelist",
        >= 1600 => "Rival",
        >= 1400 => "Challenger",
        _ => "Combatant",
    };
}

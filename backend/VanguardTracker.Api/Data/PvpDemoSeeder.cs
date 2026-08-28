using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Beta: Platzhalterdaten fürs PvP-Rating-Leiter-Dashboard. Es gibt (noch) keine
/// Blizzard-Battle.net-API-Anbindung für Ladder-Daten — deshalb bewusst rein
/// fiktive Team-/Spielernamen statt erfundener Ratings für echte Personen
/// vorzutäuschen. Sobald eine echte API-Quelle angebunden ist, ersetzt ein
/// Polling-Job (analog WarcraftLogsPollingService) diesen Seeder.
/// </summary>
public static class PvpDemoSeeder
{
    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.PvpTeams.AnyAsync()) return;

        var now = DateTimeOffset.UtcNow;

        AddBracket(db, PvpBracket.ThreeVThree, now,
            ("Ember Vanguard", "EU", 2687, new[] { "Nightglass", "Suncaller", "Vraskor" }),
            ("Frostcoil Trio", "EU", 2541, new[] { "Ashenwake", "Coldmourne", "Thundervex" }),
            ("Sable Wardens", "US", 2488, new[] { "Grimhollow", "Ravenshade", "Duskwarden" }),
            ("Voidbound Three", "US", 2312, new[] { "Nyxaria", "Shadowmere", "Emberfall" }),
            ("Ironclad Trinity", "TW", 2156, new[] { "Steelrend", "Warglory", "Ironvow" }),
            ("Wraithcall", "KR", 2034, new[] { "Hexbane", "Soulrend", "Netherquill" }),
            ("Stormforged", "EU", 1876, new[] { "Galewind", "Tempestra", "Boltcaster" }),
            ("Dawnwatch", "US", 1622, new[] { "Sunveil", "Lightbringer", "Aurorafen" }));

        AddBracket(db, PvpBracket.TwoVTwo, now,
            ("Twin Fangs", "EU", 2521, new[] { "Venomstrike", "Coldbite" }),
            ("Ashen Duo", "US", 2398, new[] { "Cinderwake", "Grimfall" }),
            ("Skyward Pair", "EU", 2201, new[] { "Windrider", "Stormsong" }),
            ("Nightfall Two", "TW", 2065, new[] { "Duskbringer", "Moonshade" }),
            ("Ironbound", "KR", 1889, new[] { "Anvilheart", "Forgewrath" }),
            ("Emberkin", "US", 1655, new[] { "Blazewing", "Pyrestep" }));

        AddBracket(db, PvpBracket.RatedBattleground, now,
            ("Crimson Battalion", "EU", 2477, RosterOf(10, "Crimson")),
            ("Northwatch Legion", "US", 2298, RosterOf(10, "Northwatch")),
            ("Ashfall Regiment", "EU", 2109, RosterOf(10, "Ashfall")),
            ("Silverpine Guard", "US", 1934, RosterOf(10, "Silverpine")),
            ("Stormcrest Company", "TW", 1748, RosterOf(10, "Stormcrest")));

        AddBracket(db, PvpBracket.SoloShuffle, now,
            ("Vex the Unseen", "EU", 2589, ["Vex the Unseen"]),
            ("Korrath Ashblade", "US", 2444, ["Korrath Ashblade"]),
            ("Mirelle Duskthorn", "EU", 2287, ["Mirelle Duskthorn"]),
            ("Baelor Stormrend", "US", 2103, ["Baelor Stormrend"]),
            ("Syvane Nightglow", "KR", 1912, ["Syvane Nightglow"]),
            ("Tharion Wolfsbane", "TW", 1701, ["Tharion Wolfsbane"]));

        await db.SaveChangesAsync();
    }

    private static string[] RosterOf(int count, string prefix) =>
        Enumerable.Range(1, count).Select(i => $"{prefix}Warden{i}").ToArray();

    private static void AddBracket(
        VanguardDbContext db,
        PvpBracket bracket,
        DateTimeOffset now,
        params (string Name, string Region, int Rating, string[] Players)[] teams)
    {
        foreach (var (name, region, rating, players) in teams)
        {
            db.PvpTeams.Add(new PvpTeam
            {
                Id = Guid.NewGuid(),
                Name = name,
                Region = region,
                Bracket = bracket,
                Rating = rating,
                PlayerNames = players.ToList(),
                UpdatedAt = now,
            });
        }
    }
}

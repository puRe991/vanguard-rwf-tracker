using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte The-Burning-Crusade-Historie. Gleiche Herkunfts-Trennung
/// wie <see cref="VanillaHistorySeeder"/>: Boss-Rosters sind dokumentierter Spiel-Content,
/// World-First-Ergebnisse (Gilde, Kill-Datum) stammen aus "The Burning Crusade Raid
/// History" — Method (https://www.method.gg/raid-history/the-burning-crusade),
/// abgerufen 2026, und werden nur übernommen, wenn die Quelle sie tatsächlich nennt.
/// Karazhan wird dort nur mit Release-Datum genannt ("Karazhan Cleared", ohne Gilde/
/// Datum) — bleibt entsprechend ohne Kill-Datensatz, offen für Community-Beiträge.
/// </summary>
public static class BurningCrusadeHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/the-burning-crusade";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "The Burning Crusade")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "The Burning Crusade",
            ReleaseDate = new DateOnly(2007, 1, 16),
        };
        var season = new Season
        {
            Id = Guid.NewGuid(),
            ExpansionId = expansion.Id,
            Number = 1,
            StartDate = expansion.ReleaseDate,
        };
        db.Expansions.Add(expansion);
        db.Seasons.Add(season);

        var nihilum = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Nihilum", "EU");
        var skGaming = await HistorySeederHelpers.GetOrAddGuildAsync(db, "SK Gaming", "EU");

        // Quelle nennt nur "Karazhan Cleared" mit Release-Datum, keine World-First-Gilde/Datum.
        HistorySeederHelpers.AddRaidWithBossesOnly(db, season.Id,
            "Karazhan",
            new DateTimeOffset(2007, 1, 28, 0, 0, 0, TimeSpan.Zero),
            [
                "Attumen the Huntsman", "Moroes", "Maiden of Virtue", "The Opera Event",
                "The Curator", "Terestian Illhoof", "Shade of Aran", "Netherspite",
                "Chess Event", "Prince Malchezaar", "Nightbane",
            ]);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Gruul's Lair",
            new DateTimeOffset(2007, 1, 16, 0, 0, 0, TimeSpan.Zero),
            ["High King Maulgar", "Gruul the Dragonkiller"],
            nihilum,
            new DateTimeOffset(2007, 2, 3, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Magtheridon's Lair",
            new DateTimeOffset(2007, 1, 16, 0, 0, 0, TimeSpan.Zero),
            ["Magtheridon"],
            nihilum,
            new DateTimeOffset(2007, 2, 24, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        // Quelle vermerkt eine zunächst "gebuggte" Nihilum-Kill-Meldung, korrigiert um
        // ein "World First Legit"-Zitat von Method — als offizielles Ergebnis bleibt
        // aber durchgehend Nihilum am 29.03.2007 stehen, kein abweichendes Datum/Gilde.
        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Serpentshrine Cavern",
            new DateTimeOffset(2007, 1, 16, 0, 0, 0, TimeSpan.Zero),
            [
                "Hydross the Unstable", "The Lurker Below", "Leotheras the Blind",
                "Fathom-Lord Karathress", "Morogrim Tidewalker", "Lady Vashj",
            ],
            nihilum,
            new DateTimeOffset(2007, 3, 29, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Tempest Keep: The Eye",
            new DateTimeOffset(2007, 1, 16, 0, 0, 0, TimeSpan.Zero),
            ["Al'ar", "Void Reaver", "High Astromancer Solarian", "Kael'thas Sunstrider"],
            nihilum,
            new DateTimeOffset(2007, 5, 25, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Black Temple",
            new DateTimeOffset(2007, 5, 22, 0, 0, 0, TimeSpan.Zero),
            [
                "High Warlord Naj'entus", "Supremus", "Shade of Akama", "Teron Gorefiend",
                "Gurtogg Bloodboil", "Reliquary of Souls", "Mother Shahraz",
                "The Illidari Council", "Illidan Stormrage",
            ],
            nihilum,
            new DateTimeOffset(2007, 6, 5, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Mount Hyjal",
            new DateTimeOffset(2007, 5, 22, 0, 0, 0, TimeSpan.Zero),
            ["Rage Winterchill", "Anetheron", "Kaz'rogal", "Azgalor", "Archimonde"],
            nihilum,
            new DateTimeOffset(2007, 6, 9, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Sunwell Plateau",
            new DateTimeOffset(2008, 3, 25, 0, 0, 0, TimeSpan.Zero),
            ["Kalecgos", "Brutallus", "Felmyst", "Eredar Twins", "M'uru", "Kil'jaeden"],
            skGaming,
            new DateTimeOffset(2008, 5, 25, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

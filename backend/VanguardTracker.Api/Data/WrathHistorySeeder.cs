using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Wrath-of-the-Lich-King-Historie. Gleiche Herkunfts-Trennung
/// wie <see cref="VanillaHistorySeeder"/>: Boss-Rosters sind dokumentierter Spiel-Content,
/// World-First-Ergebnisse (Gilde, Kill-Datum) stammen aus "Wrath of the Lich King Raid
/// History" — Method (https://www.method.gg/raid-history/wrath-of-the-lich-king),
/// abgerufen 2026. Vault of Archavon wird dort ohne Boss-/Gilden-/Datumsangabe genannt
/// und bleibt entsprechend ohne Kill-Datensatz, offen für Community-Beiträge.
/// </summary>
public static class WrathHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/wrath-of-the-lich-king";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Wrath of the Lich King")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Wrath of the Lich King",
            ReleaseDate = new DateOnly(2008, 11, 13),
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

        var ensidia = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Ensidia", "EU");
        var paragon = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Paragon", "EU");
        var stars = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Stars", "TW");
        var premonition = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Premonition", "US");

        // Quelle nennt keinen Boss/keine Gilde/kein Datum für Vault of Archavon.
        HistorySeederHelpers.AddRaidWithBossesOnly(db, season.Id,
            "Vault of Archavon",
            new DateTimeOffset(2008, 11, 13, 0, 0, 0, TimeSpan.Zero),
            ["Archavon the Stone Watcher"]);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Naxxramas",
            new DateTimeOffset(2008, 11, 13, 0, 0, 0, TimeSpan.Zero),
            [
                "Anub'Rekhan", "Grand Widow Faerlina", "Maexxna",
                "Noth the Plaguebringer", "Heigan the Unclean", "Loatheb",
                "Instructor Razuvious", "Gothik the Harvester", "The Four Horsemen",
                "Patchwerk", "Grobbulus", "Gluth", "Thaddius",
                "Sapphiron", "Kel'Thuzad",
            ],
            ensidia,
            new DateTimeOffset(2008, 11, 15, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "The Obsidian Sanctum",
            new DateTimeOffset(2008, 11, 13, 0, 0, 0, TimeSpan.Zero),
            ["Sartharion"],
            ensidia,
            new DateTimeOffset(2008, 11, 21, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "The Eye of Eternity",
            new DateTimeOffset(2008, 11, 13, 0, 0, 0, TimeSpan.Zero),
            ["Malygos"],
            ensidia,
            new DateTimeOffset(2008, 11, 15, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        // Algalon the Observer bewusst ausgelassen: optionaler, per Wochenquest
        // freigeschalteter Geheim-Boss, nicht Teil des regulären Tier-Clears —
        // die Quelle nennt Yogg-Saron als finalen Boss/World-First-Ergebnis.
        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Ulduar",
            new DateTimeOffset(2009, 4, 14, 0, 0, 0, TimeSpan.Zero),
            [
                "Flame Leviathan", "Ignis the Furnace Master", "Razorscale", "XT-002 Deconstructor",
                "Assembly of Iron", "Kologarn", "Auriaya", "Hodir", "Thorim", "Freya",
                "Mimiron", "General Vezax", "Yogg-Saron",
            ],
            stars,
            new DateTimeOffset(2009, 7, 7, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Trial of the Grand Crusader",
            new DateTimeOffset(2009, 8, 4, 0, 0, 0, TimeSpan.Zero),
            ["Northrend Beasts", "Lord Jaraxxus", "Faction Champions", "Val'kyr Twins", "Anub'arak"],
            paragon,
            new DateTimeOffset(2009, 9, 7, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Icecrown Citadel",
            new DateTimeOffset(2009, 12, 8, 0, 0, 0, TimeSpan.Zero),
            [
                "Lord Marrowgar", "Lady Deathwhisper", "Gunship Battle", "Deathbringer Saurfang",
                "Festergut", "Rotface", "Professor Putricide", "Blood Prince Council",
                "Blood-Queen Lana'thel", "Valithria Dreamwalker", "Sindragosa", "The Lich King",
            ],
            paragon,
            new DateTimeOffset(2010, 3, 26, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Ruby Sanctum",
            new DateTimeOffset(2010, 6, 30, 0, 0, 0, TimeSpan.Zero),
            ["Halion"],
            premonition,
            new DateTimeOffset(2010, 6, 30, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

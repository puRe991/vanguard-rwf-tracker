using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Vanilla-Historie — es gibt für diese Ära keine
/// verlässliche API (Warcraft Logs deckt erst ab ca. Cataclysm zuverlässig ab),
/// daher stammen die Daten aus einer kuratierten Community-Quelle statt aus einer
/// API-Anbindung.
///
/// Zwei getrennte Herkünfte, nicht zu verwechseln:
/// - Boss-Rosters (Reihenfolge, Namen) sind öffentlich dokumentierter Spiel-Content,
///   keine strittigen/quellenpflichtigen Fakten.
/// - World-First-Ergebnisse (Gilde, Kill-Datum) stammen aus "Vanilla Raid History
///   of World Firsts in World of Warcraft" — Method (https://www.method.gg/raid-history),
///   abgerufen 2026, und werden nur übernommen, wenn die Quelle sie tatsächlich nennt.
///   Für Zul'Gurub und Ruins of Ahn'Qiraj liefert die Quelle keine Gilden-/Datumsangaben
///   — diese Raid-Tiers bleiben ohne Kill-Datensatz und warten auf Community-Beiträge
///   über den Submit-Kill-Workflow (Moderation vor Veröffentlichung). Pull-Zahlen sind
///   für die Vanilla-Ära generell nicht überliefert.
/// </summary>
public static class VanillaHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Classic")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Classic",
            ReleaseDate = new DateOnly(2004, 11, 23),
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

        var ruined = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Ruined", "US");
        var ascent = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Ascent", "US");
        var drama = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Drama", "US");
        var nihilum = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Nihilum", "EU");

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Onyxia's Lair",
            new DateTimeOffset(2004, 11, 23, 0, 0, 0, TimeSpan.Zero),
            ["Onyxia"],
            ruined,
            new DateTimeOffset(2005, 1, 30, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Molten Core",
            new DateTimeOffset(2004, 11, 23, 0, 0, 0, TimeSpan.Zero),
            [
                "Lucifron", "Magmadar", "Gehennas", "Garr", "Baron Geddon",
                "Shazzrah", "Sulfuron Harbinger", "Golemagg the Incinerator",
                "Majordomo Executus", "Ragnaros",
            ],
            ascent,
            new DateTimeOffset(2005, 4, 25, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Blackwing Lair",
            new DateTimeOffset(2005, 7, 12, 0, 0, 0, TimeSpan.Zero),
            [
                "Razorgore the Untamed", "Vaelastrasz the Corrupt", "Broodlord Lashlayer",
                "Firemaw", "Ebonroc", "Flamegor", "Chromaggus", "Nefarian",
            ],
            drama,
            new DateTimeOffset(2005, 9, 26, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        // Zul'Gurub: Quelle nennt nur das Release-Datum, keine World-First-Gilde/Kill-Datum.
        HistorySeederHelpers.AddRaidWithBossesOnly(db, season.Id,
            "Zul'Gurub",
            new DateTimeOffset(2005, 9, 13, 0, 0, 0, TimeSpan.Zero),
            [
                "High Priestess Jeklik", "High Priest Venoxis", "High Priestess Mar'li",
                "Bloodlord Mandokir", "Gahz'rilla", "Wushoolay", "Renataki", "Hazza'rah",
                "High Priest Thekal", "High Priestess Arlokk", "Jin'do the Hexxer",
                "Hakkar the Soulflayer",
            ]);

        // Ruins of Ahn'Qiraj (AQ20): in der Quelle nicht mit Datum/Gilde belegt.
        HistorySeederHelpers.AddRaidWithBossesOnly(db, season.Id,
            "Ruins of Ahn'Qiraj",
            new DateTimeOffset(2006, 1, 3, 0, 0, 0, TimeSpan.Zero),
            [
                "Kurinnaxx", "General Rajaxx", "Moam", "Buru the Gorger",
                "Ayamiss the Hunter", "Ossirian the Unscarred",
            ]);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Temple of Ahn'Qiraj",
            new DateTimeOffset(2006, 1, 3, 0, 0, 0, TimeSpan.Zero),
            [
                "The Prophet Skeram", "Lord Kri", "Princess Yauj", "Vem",
                "Battleguard Sartura", "Fankriss the Unyielding", "Viscidus",
                "Princess Huhuran", "Twin Emperors Vek'lor and Veknilash", "Ouro", "C'Thun",
            ],
            nihilum,
            new DateTimeOffset(2006, 4, 25, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Naxxramas",
            new DateTimeOffset(2006, 6, 20, 0, 0, 0, TimeSpan.Zero),
            [
                "Anub'Rekhan", "Grand Widow Faerlina", "Maexxna",
                "Noth the Plaguebringer", "Heigan the Unclean", "Loatheb",
                "Instructor Razuvious", "Gothik the Harvester", "The Four Horsemen",
                "Patchwerk", "Grobbulus", "Gluth", "Thaddius",
                "Sapphiron", "Kel'Thuzad",
            ],
            nihilum,
            new DateTimeOffset(2006, 9, 7, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

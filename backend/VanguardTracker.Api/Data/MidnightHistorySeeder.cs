using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Midnight-Historie. Quelle: "Midnight Raid History" —
/// Method (https://www.method.gg/raid-history/midnight), abgerufen 2026.
/// Boss-für-Boss-Weltrekorde wie bei <see cref="MistsOfPandariaHistorySeeder"/>.
///
/// Erstes Addon mit echten Season-1/Season-2-Angaben in der Quelle statt nur einer
/// durchgehenden Season pro Addon. Season 2 (The Venomous Abyss) läuft laut Quelle
/// noch ("Status: In Progress") — Boss-Roster ist bekannt, World-Firsts noch nicht,
/// daher ohne Kill-Datensätze angelegt statt Ergebnisse zu erfinden.
/// </summary>
public static class MidnightHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/midnight";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Midnight")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Midnight",
            ReleaseDate = new DateOnly(2026, 3, 24),
        };
        var season1 = new Season
        {
            Id = Guid.NewGuid(),
            ExpansionId = expansion.Id,
            Number = 1,
            StartDate = new DateOnly(2026, 3, 24),
        };
        var season2 = new Season
        {
            Id = Guid.NewGuid(),
            ExpansionId = expansion.Id,
            Number = 2,
            StartDate = new DateOnly(2026, 8, 18),
        };
        db.Expansions.Add(expansion);
        db.Seasons.AddRange(season1, season2);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season1.Id,
            "The Voidspire",
            new DateTimeOffset(2026, 3, 24, 0, 0, 0, TimeSpan.Zero),
            [
                ("Imperator Averzian", "Melee Mechanics", "US", new DateTimeOffset(2026, 3, 24, 0, 0, 0, TimeSpan.Zero)),
                ("Vorasius", "Nurfed", "US", new DateTimeOffset(2026, 3, 25, 0, 0, 0, TimeSpan.Zero)),
                ("Fallen-King Salhadaar", "Liquid", "US", new DateTimeOffset(2026, 3, 26, 0, 0, 0, TimeSpan.Zero)),
                ("Vaelgor & Ezzorak", "Liquid", "US", new DateTimeOffset(2026, 3, 26, 0, 0, 0, TimeSpan.Zero)),
                ("Lightblinded Vanguard", "Liquid", "US", new DateTimeOffset(2026, 3, 26, 0, 0, 0, TimeSpan.Zero)),
                ("Crown of the Cosmos", "Liquid", "US", new DateTimeOffset(2026, 3, 27, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season1.Id,
            "The Dreamrift",
            new DateTimeOffset(2026, 3, 24, 0, 0, 0, TimeSpan.Zero),
            [
                ("Chimaerus, the Undreamt God", "Liquid", "US", new DateTimeOffset(2026, 3, 25, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season1.Id,
            "March on Quel'Danas",
            new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero),
            [
                ("Belo'ren, Child of Al'ar", "Liquid", "US", new DateTimeOffset(2026, 4, 3, 0, 0, 0, TimeSpan.Zero)),
                ("Midnight Falls (L'ura)", "Liquid", "US", new DateTimeOffset(2026, 4, 6, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        // The Venomous Abyss: laut Quelle noch "In Progress" — nur das Roster ist
        // bekannt, keine World-Firsts. Bewusst ohne Kill-Datensätze, siehe Klassen-Kommentar.
        HistorySeederHelpers.AddRaidWithBossesOnly(db, season2.Id,
            "The Venomous Abyss",
            new DateTimeOffset(2026, 8, 18, 0, 0, 0, TimeSpan.Zero),
            [
                "Nek'zali the Soulcoiler", "Entombed Sentinels", "The Lost Explorers",
                "Vashnik the Malignant", "Sszorak", "The Twin Fangs",
                "The Coiled Altar", "Ula'tek",
            ]);

        await db.SaveChangesAsync();
    }
}

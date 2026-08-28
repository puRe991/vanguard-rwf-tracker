using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Cataclysm-Historie. Gleiche Herkunfts-Trennung wie
/// <see cref="VanillaHistorySeeder"/>: Boss-Rosters sind dokumentierter Spiel-Content,
/// World-First-Ergebnisse (Gilde, Kill-Datum) stammen aus "Cataclysm Raid History" —
/// Method (https://www.method.gg/raid-history/cataclysm), abgerufen 2026. Baradin Hold
/// wird dort ohne Gilden-/Datumsangabe genannt und bleibt entsprechend ohne
/// Kill-Datensatz, offen für Community-Beiträge.
/// </summary>
public static class CataclysmHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/cataclysm";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Cataclysm")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Cataclysm",
            ReleaseDate = new DateOnly(2010, 12, 7),
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

        var paragon = await HistorySeederHelpers.GetOrAddGuildAsync(db, "Paragon", "EU");
        var kinRaiders = await HistorySeederHelpers.GetOrAddGuildAsync(db, "KIN Raiders", "KR");

        // Quelle nennt keine Gilde/kein Datum für Baradin Hold.
        HistorySeederHelpers.AddRaidWithBossesOnly(db, season.Id,
            "Baradin Hold",
            new DateTimeOffset(2010, 12, 7, 0, 0, 0, TimeSpan.Zero),
            ["Argaloth"]);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Blackwing Descent",
            new DateTimeOffset(2010, 12, 7, 0, 0, 0, TimeSpan.Zero),
            [
                "Magmaw", "Omnotron Defense System", "Maloriak",
                "Atramedes", "Chimaeron", "Nefarian",
            ],
            paragon,
            new DateTimeOffset(2011, 1, 9, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "The Bastion of Twilight",
            new DateTimeOffset(2010, 12, 7, 0, 0, 0, TimeSpan.Zero),
            ["Halfus Wyrmbreaker", "Valiona and Theralion", "Ascendant Council", "Cho'gall", "Sinestra"],
            paragon,
            new DateTimeOffset(2011, 1, 20, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Throne of the Four Winds",
            new DateTimeOffset(2010, 12, 7, 0, 0, 0, TimeSpan.Zero),
            ["Conclave of Wind", "Al'Akir"],
            paragon,
            new DateTimeOffset(2011, 1, 24, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Firelands",
            new DateTimeOffset(2011, 6, 28, 0, 0, 0, TimeSpan.Zero),
            [
                "Beth'tilac", "Lord Rhyolith", "Alysrazor", "Shannox",
                "Baleroc, the Gatekeeper", "Majordomo Staghelm", "Ragnaros",
            ],
            paragon,
            new DateTimeOffset(2011, 7, 19, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        HistorySeederHelpers.AddRaidWithFinalBossKill(db, season.Id,
            "Dragon Soul",
            new DateTimeOffset(2011, 11, 29, 0, 0, 0, TimeSpan.Zero),
            [
                "Morchok", "Warlord Zon'ozz", "Yor'sahj the Unsleeping", "Hagara the Stormbinder",
                "Ultraxion", "Warmaster Blackhorn", "Spine of Deathwing", "Madness of Deathwing",
            ],
            kinRaiders,
            new DateTimeOffset(2011, 12, 20, 0, 0, 0, TimeSpan.Zero),
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte The-War-Within-Historie (die echte, nicht die
/// fiktive Demo-Season aus <see cref="DbSeeder"/>). Quelle: "The War Within Raid
/// History" — Method (https://www.method.gg/raid-history/the-war-within), abgerufen
/// 2026. Boss-für-Boss-Weltrekorde wie bei <see cref="MistsOfPandariaHistorySeeder"/>.
/// </summary>
public static class TheWarWithinHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/the-war-within";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "The War Within")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "The War Within",
            ReleaseDate = new DateOnly(2024, 8, 26),
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

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Nerub'ar Palace",
            new DateTimeOffset(2024, 9, 17, 0, 0, 0, TimeSpan.Zero),
            [
                ("Ulgrax the Devourer", "Melee Mechanics", "US", new DateTimeOffset(2024, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("The Bloodbound Horror", "Melee Mechanics", "US", new DateTimeOffset(2024, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Sikran, Captain of the Sureki", "Liquid", "US", new DateTimeOffset(2024, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Rasha'nan", "Liquid", "US", new DateTimeOffset(2024, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Broodtwister Ovi'nax", "Liquid", "US", new DateTimeOffset(2024, 9, 20, 0, 0, 0, TimeSpan.Zero)),
                ("Nexus-Princess Ky'veza", "Liquid", "US", new DateTimeOffset(2024, 9, 22, 0, 0, 0, TimeSpan.Zero)),
                ("The Silken Court", "Liquid", "US", new DateTimeOffset(2024, 9, 24, 0, 0, 0, TimeSpan.Zero)),
                ("Queen Ansurek", "Liquid", "US", new DateTimeOffset(2024, 9, 29, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Liberation of Undermine",
            new DateTimeOffset(2025, 3, 4, 0, 0, 0, TimeSpan.Zero),
            [
                ("Vexie and the Geargrinders", "Melee Mechanics", "US", new DateTimeOffset(2025, 3, 4, 0, 0, 0, TimeSpan.Zero)),
                ("Cauldron of Carnage", "Bound", "US", new DateTimeOffset(2025, 3, 7, 0, 0, 0, TimeSpan.Zero)),
                ("Rik Reverb", "Instant Dollars", "US", new DateTimeOffset(2025, 3, 8, 0, 0, 0, TimeSpan.Zero)),
                ("Stix Bunkjunker", "Liquid", "US", new DateTimeOffset(2025, 3, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Sprocketmonger Lockenstock", "Echo", "EU", new DateTimeOffset(2025, 3, 11, 0, 0, 0, TimeSpan.Zero)),
                ("The One-Armed Bandit", "Liquid", "US", new DateTimeOffset(2025, 3, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Mug'Zee, Heads of Security", "Liquid", "US", new DateTimeOffset(2025, 3, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Chrome King Gallywix", "Liquid", "US", new DateTimeOffset(2025, 3, 16, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Manaforge Omega",
            new DateTimeOffset(2025, 8, 12, 0, 0, 0, TimeSpan.Zero),
            [
                ("Plexus Sentinel", "Consequence", "US", new DateTimeOffset(2025, 8, 14, 0, 0, 0, TimeSpan.Zero)),
                ("Soulbinder Naazindhri", "SOMA", "US", new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Loom'ithar", "Instant Dollars", "US", new DateTimeOffset(2025, 8, 16, 0, 0, 0, TimeSpan.Zero)),
                ("Forgeweaver Araz", "Liquid", "US", new DateTimeOffset(2025, 8, 17, 0, 0, 0, TimeSpan.Zero)),
                ("The Soul Hunters", "Liquid", "US", new DateTimeOffset(2025, 8, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Fractillus", "Echo", "EU", new DateTimeOffset(2025, 8, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Nexus-King Salhadaar", "Liquid", "US", new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero)),
                ("Dimensius, the All-Devouring", "Liquid", "US", new DateTimeOffset(2025, 8, 24, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Battle-for-Azeroth-Historie. Quelle: "Battle for
/// Azeroth Raid History" — Method (https://www.method.gg/raid-history/battle-for-azeroth),
/// abgerufen 2026. Boss-für-Boss-Weltrekorde wie bei
/// <see cref="MistsOfPandariaHistorySeeder"/>. Battle of Dazar'alor hat pro Fraktion
/// leicht unterschiedliche erste Bosse (Champion of the Light/Alliance vs.
/// Grong/Horde) — beide werden als eigene Einträge übernommen, wie von der Quelle
/// gelistet, statt eine Fraktion zu unterschlagen.
/// </summary>
public static class BattleForAzerothHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/battle-for-azeroth";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Battle for Azeroth")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Battle for Azeroth",
            ReleaseDate = new DateOnly(2018, 8, 14),
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
            "Uldir",
            new DateTimeOffset(2018, 9, 11, 0, 0, 0, TimeSpan.Zero),
            [
                ("Taloc the Corrupted", "Big Dumb Guild", "US", new DateTimeOffset(2018, 9, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Mother", "Big Dumb Guild", "US", new DateTimeOffset(2018, 9, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Zek'voz, Herald of N'zoth", "Limit", "US", new DateTimeOffset(2018, 9, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Vectis", "Big Dumb Guild", "US", new DateTimeOffset(2018, 9, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Fetid Devourer", "Limit", "US", new DateTimeOffset(2018, 9, 14, 0, 0, 0, TimeSpan.Zero)),
                ("Zul, Reborn", "Method", "EU", new DateTimeOffset(2018, 9, 14, 0, 0, 0, TimeSpan.Zero)),
                ("Mythrax the Unraveler", "Limit", "US", new DateTimeOffset(2018, 9, 16, 0, 0, 0, TimeSpan.Zero)),
                ("G'huun", "Method", "EU", new DateTimeOffset(2018, 9, 19, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Battle of Dazar'alor",
            new DateTimeOffset(2019, 1, 29, 0, 0, 0, TimeSpan.Zero),
            [
                ("Champion of the Light", "Big Dumb Guild", "US", new DateTimeOffset(2019, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Grong", "Wildcard Gaming", "US", new DateTimeOffset(2019, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Jadefire Masters", "Big Dumb Guild", "US", new DateTimeOffset(2019, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Opulence", "Limit", "US", new DateTimeOffset(2019, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Conclave of the Chosen", "Limit", "US", new DateTimeOffset(2019, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("King Rastakhan", "Limit", "US", new DateTimeOffset(2019, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("High Tinker Mekkatorque", "Limit", "US", new DateTimeOffset(2019, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Stormwall Blockade", "Limit", "US", new DateTimeOffset(2019, 1, 30, 0, 0, 0, TimeSpan.Zero)),
                ("Lady Jaina Proudmoore", "Method", "EU", new DateTimeOffset(2019, 2, 5, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Crucible of Storms",
            new DateTimeOffset(2019, 4, 23, 0, 0, 0, TimeSpan.Zero),
            [
                ("The Restless Cabal", "Pieces", "EU", new DateTimeOffset(2019, 4, 24, 0, 0, 0, TimeSpan.Zero)),
                ("Uu'nat, Harbinger of the Void", "Pieces", "EU", new DateTimeOffset(2019, 5, 3, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "The Eternal Palace",
            new DateTimeOffset(2019, 7, 16, 0, 0, 0, TimeSpan.Zero),
            [
                ("Abyssal Commander Sivara", "Limit", "US", new DateTimeOffset(2019, 7, 16, 0, 0, 0, TimeSpan.Zero)),
                ("Blackwater Behemoth", "Big Dumb Guild", "US", new DateTimeOffset(2019, 7, 16, 0, 0, 0, TimeSpan.Zero)),
                ("Radiance of Azshara", "Limit", "US", new DateTimeOffset(2019, 7, 16, 0, 0, 0, TimeSpan.Zero)),
                ("Lady Ashvane", "Method", "EU", new DateTimeOffset(2019, 7, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Orgozoa", "Limit", "US", new DateTimeOffset(2019, 7, 18, 0, 0, 0, TimeSpan.Zero)),
                ("The Queen's Court", "Method", "EU", new DateTimeOffset(2019, 7, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Za'qul", "Limit", "US", new DateTimeOffset(2019, 7, 20, 0, 0, 0, TimeSpan.Zero)),
                ("Queen Azshara", "Method", "EU", new DateTimeOffset(2019, 7, 28, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Ny'alotha, the Waking City",
            new DateTimeOffset(2020, 1, 28, 0, 0, 0, TimeSpan.Zero),
            [
                ("Wrathion", "Midwinter", "US", new DateTimeOffset(2020, 1, 28, 0, 0, 0, TimeSpan.Zero)),
                ("Maut", "Midwinter", "US", new DateTimeOffset(2020, 1, 28, 0, 0, 0, TimeSpan.Zero)),
                ("The Prophet Skitra", "Complexity Limit", "US", new DateTimeOffset(2020, 1, 28, 0, 0, 0, TimeSpan.Zero)),
                ("Dark Inquisitor Xanesh", "Big Dumb Guild", "US", new DateTimeOffset(2020, 1, 28, 0, 0, 0, TimeSpan.Zero)),
                ("The Hivemind", "Complexity Limit", "US", new DateTimeOffset(2020, 1, 28, 0, 0, 0, TimeSpan.Zero)),
                ("Shad'har the Insatiable", "Complexity Limit", "US", new DateTimeOffset(2020, 1, 28, 0, 0, 0, TimeSpan.Zero)),
                ("Drest'agath", "Complexity Limit", "US", new DateTimeOffset(2020, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Vexiona", "Complexity Limit", "US", new DateTimeOffset(2020, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Ra-den the Despoiled", "Complexity Limit", "US", new DateTimeOffset(2020, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Il'gynoth", "Complexity Limit", "US", new DateTimeOffset(2020, 1, 31, 0, 0, 0, TimeSpan.Zero)),
                ("Carapace of N'Zoth", "Complexity Limit", "US", new DateTimeOffset(2020, 2, 1, 0, 0, 0, TimeSpan.Zero)),
                ("N'Zoth the Corruptor", "Complexity Limit", "US", new DateTimeOffset(2020, 2, 6, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

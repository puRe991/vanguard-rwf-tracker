using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Warlords-of-Draenor-Historie. Quelle: "Warlords of
/// Draenor Raid History" — Method (https://www.method.gg/raid-history/warlords-of-draenor),
/// abgerufen 2026. Boss-für-Boss-Weltrekorde wie bei
/// <see cref="MistsOfPandariaHistorySeeder"/>.
/// </summary>
public static class WarlordsHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/warlords-of-draenor";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Warlords of Draenor")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Warlords of Draenor",
            ReleaseDate = new DateOnly(2014, 11, 13),
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
            "Highmaul",
            new DateTimeOffset(2014, 12, 2, 0, 0, 0, TimeSpan.Zero),
            [
                ("Kargath Bladefist", "Ascension", "OC", new DateTimeOffset(2014, 12, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Brackenspore", "Ascension", "OC", new DateTimeOffset(2014, 12, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Twin Ogron", "Ascension", "OC", new DateTimeOffset(2014, 12, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Ko'ragh", "Ascension", "OC", new DateTimeOffset(2014, 12, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Tectus", "Midwinter", "US", new DateTimeOffset(2014, 12, 10, 0, 0, 0, TimeSpan.Zero)),
                ("The Butcher", "Method", "EU", new DateTimeOffset(2014, 12, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Imperator Mar'gok", "Paragon", "EU", new DateTimeOffset(2014, 12, 13, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Blackrock Foundry",
            new DateTimeOffset(2015, 2, 3, 0, 0, 0, TimeSpan.Zero),
            [
                ("Oregorger the Devourer", "Ascension", "OC", new DateTimeOffset(2015, 2, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Gruul", "Midwinter", "US", new DateTimeOffset(2015, 2, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Hans'gar & Franzok", "Blood Legion", "US", new DateTimeOffset(2015, 2, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Beastlord Darmac", "Blood Legion", "US", new DateTimeOffset(2015, 2, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Flamebender Ka'graz", "Midwinter", "US", new DateTimeOffset(2015, 2, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Operator Thogar", "Blood Legion", "US", new DateTimeOffset(2015, 2, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Kromog", "Method", "EU", new DateTimeOffset(2015, 2, 12, 0, 0, 0, TimeSpan.Zero)),
                ("The Iron Maidens", "Midwinter", "US", new DateTimeOffset(2015, 2, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Blast Furnace", "Method", "EU", new DateTimeOffset(2015, 2, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Blackhand", "Method", "EU", new DateTimeOffset(2015, 2, 20, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Hellfire Citadel",
            new DateTimeOffset(2015, 6, 23, 0, 0, 0, TimeSpan.Zero),
            [
                ("Hellfire Assault", "Limit", "US", new DateTimeOffset(2015, 6, 30, 0, 0, 0, TimeSpan.Zero)),
                ("Iron Reaver", "Limit", "US", new DateTimeOffset(2015, 6, 30, 0, 0, 0, TimeSpan.Zero)),
                ("Kormrok", "Limit", "US", new DateTimeOffset(2015, 6, 30, 0, 0, 0, TimeSpan.Zero)),
                ("Hellfire High Council", "Ascension", "OC", new DateTimeOffset(2015, 7, 1, 0, 0, 0, TimeSpan.Zero)),
                ("Kilrogg Deadeye", "Ascension", "OC", new DateTimeOffset(2015, 7, 1, 0, 0, 0, TimeSpan.Zero)),
                ("Gorefiend", "Method", "EU", new DateTimeOffset(2015, 7, 2, 0, 0, 0, TimeSpan.Zero)),
                ("Shadow-Lord Iskar", "Paragon", "EU", new DateTimeOffset(2015, 7, 2, 0, 0, 0, TimeSpan.Zero)),
                ("Fel Lord Zakuun", "Method", "EU", new DateTimeOffset(2015, 7, 2, 0, 0, 0, TimeSpan.Zero)),
                ("Socrethar the Eternal", "Method", "EU", new DateTimeOffset(2015, 7, 2, 0, 0, 0, TimeSpan.Zero)),
                ("Tyrant Velhari", "Paragon", "EU", new DateTimeOffset(2015, 7, 4, 0, 0, 0, TimeSpan.Zero)),
                ("Xhul'horac", "Method", "EU", new DateTimeOffset(2015, 7, 5, 0, 0, 0, TimeSpan.Zero)),
                ("Mannoroth", "Method", "EU", new DateTimeOffset(2015, 7, 7, 0, 0, 0, TimeSpan.Zero)),
                ("Archimonde", "Method", "EU", new DateTimeOffset(2015, 7, 16, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

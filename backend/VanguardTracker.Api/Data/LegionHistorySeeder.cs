using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Legion-Historie. Quelle: "Legion Raid History" —
/// Method (https://www.method.gg/raid-history/legion), abgerufen 2026.
/// Boss-für-Boss-Weltrekorde wie bei <see cref="MistsOfPandariaHistorySeeder"/>.
/// </summary>
public static class LegionHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/legion";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Legion")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Legion",
            ReleaseDate = new DateOnly(2016, 8, 30),
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
            "The Emerald Nightmare",
            new DateTimeOffset(2016, 9, 20, 0, 0, 0, TimeSpan.Zero),
            [
                ("Nythendra", "Easy", "US", new DateTimeOffset(2016, 9, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Elerethe Renferal", "Midwinter", "US", new DateTimeOffset(2016, 9, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Ursoc", "Midwinter", "US", new DateTimeOffset(2016, 9, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Dragons of Nightmare", "Midwinter", "US", new DateTimeOffset(2016, 9, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Il'gynoth", "Exorsus", "EU", new DateTimeOffset(2016, 9, 28, 0, 0, 0, TimeSpan.Zero)),
                ("Cenarius", "Exorsus", "EU", new DateTimeOffset(2016, 9, 28, 0, 0, 0, TimeSpan.Zero)),
                ("Xavius", "Exorsus", "EU", new DateTimeOffset(2016, 9, 29, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Trial of Valor",
            new DateTimeOffset(2016, 11, 8, 0, 0, 0, TimeSpan.Zero),
            [
                ("Odyn", "SNF", "US", new DateTimeOffset(2016, 11, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Guarm", "Limit", "US", new DateTimeOffset(2016, 11, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Helya", "Method", "EU", new DateTimeOffset(2016, 11, 18, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "The Nighthold",
            new DateTimeOffset(2017, 1, 18, 0, 0, 0, TimeSpan.Zero),
            [
                ("Skorpyron", "SNF", "US", new DateTimeOffset(2017, 1, 24, 0, 0, 0, TimeSpan.Zero)),
                ("Chronomatic Anomaly", "SNF", "US", new DateTimeOffset(2017, 1, 24, 0, 0, 0, TimeSpan.Zero)),
                ("Trilliax", "SNF", "US", new DateTimeOffset(2017, 1, 24, 0, 0, 0, TimeSpan.Zero)),
                ("Spellblade Aluriel", "Limit", "US", new DateTimeOffset(2017, 1, 24, 0, 0, 0, TimeSpan.Zero)),
                ("Tichondrius", "Limit", "US", new DateTimeOffset(2017, 1, 25, 0, 0, 0, TimeSpan.Zero)),
                ("Krosus", "Limit", "US", new DateTimeOffset(2017, 1, 25, 0, 0, 0, TimeSpan.Zero)),
                ("High-Botanist Tel'arn", "Serenity", "EU", new DateTimeOffset(2017, 1, 25, 0, 0, 0, TimeSpan.Zero)),
                ("Star-Augur Etraeus", "Serenity", "EU", new DateTimeOffset(2017, 1, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Grand-Magistrix Elisande", "Serenity", "EU", new DateTimeOffset(2017, 1, 30, 0, 0, 0, TimeSpan.Zero)),
                ("Gul'dan", "Exorsus", "EU", new DateTimeOffset(2017, 2, 4, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Tomb of Sargeras",
            new DateTimeOffset(2017, 6, 20, 0, 0, 0, TimeSpan.Zero),
            [
                ("Goroth", "Easy", "US", new DateTimeOffset(2017, 6, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Demonic Inquisition", "Easy", "US", new DateTimeOffset(2017, 6, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Harjatan", "Big Dumb Guild", "US", new DateTimeOffset(2017, 6, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Sisters of the Moon", "Big Dumb Guild", "US", new DateTimeOffset(2017, 6, 27, 0, 0, 0, TimeSpan.Zero)),
                ("Mistress Sassz'ine", "Method", "EU", new DateTimeOffset(2017, 6, 28, 0, 0, 0, TimeSpan.Zero)),
                ("The Desolate Host", "Big Dumb Guild", "US", new DateTimeOffset(2017, 6, 28, 0, 0, 0, TimeSpan.Zero)),
                ("Maiden of Vigilance", "Method", "EU", new DateTimeOffset(2017, 6, 29, 0, 0, 0, TimeSpan.Zero)),
                ("Fallen Avatar", "Method", "EU", new DateTimeOffset(2017, 7, 4, 0, 0, 0, TimeSpan.Zero)),
                ("Kil'jaeden", "Method", "EU", new DateTimeOffset(2017, 7, 16, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Antorus, the Burning Throne",
            new DateTimeOffset(2017, 11, 28, 0, 0, 0, TimeSpan.Zero),
            [
                ("Garothi Worldbreaker", "Big Dumb Guild", "US", new DateTimeOffset(2017, 12, 5, 0, 0, 0, TimeSpan.Zero)),
                ("Felhounds of Sargeras", "Limit", "US", new DateTimeOffset(2017, 12, 5, 0, 0, 0, TimeSpan.Zero)),
                ("Portal Keeper Hasabel", "Limit", "US", new DateTimeOffset(2017, 12, 5, 0, 0, 0, TimeSpan.Zero)),
                ("Antoran High Command", "Limit", "US", new DateTimeOffset(2017, 12, 5, 0, 0, 0, TimeSpan.Zero)),
                ("Eonar the Life-Binder", "Limit", "US", new DateTimeOffset(2017, 12, 5, 0, 0, 0, TimeSpan.Zero)),
                ("Imonar the Soulhunter", "Limit", "US", new DateTimeOffset(2017, 12, 5, 0, 0, 0, TimeSpan.Zero)),
                ("Kin'garoth", "Limit", "US", new DateTimeOffset(2017, 12, 5, 0, 0, 0, TimeSpan.Zero)),
                ("Varimathras", "Limit", "US", new DateTimeOffset(2017, 12, 6, 0, 0, 0, TimeSpan.Zero)),
                ("The Coven of Shivarra", "Limit", "US", new DateTimeOffset(2017, 12, 6, 0, 0, 0, TimeSpan.Zero)),
                ("Aggramar", "Method", "EU", new DateTimeOffset(2017, 12, 6, 0, 0, 0, TimeSpan.Zero)),
                ("Argus the Unmaker", "Method", "EU", new DateTimeOffset(2017, 12, 13, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Shadowlands-Historie. Quelle: "Shadowlands Raid
/// History" — Method (https://www.method.gg/raid-history/shadowlands), abgerufen 2026.
/// Boss-für-Boss-Weltrekorde wie bei <see cref="MistsOfPandariaHistorySeeder"/>.
/// </summary>
public static class ShadowlandsHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/shadowlands";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Shadowlands")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Shadowlands",
            ReleaseDate = new DateOnly(2020, 11, 23),
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
            "Castle Nathria",
            new DateTimeOffset(2020, 12, 15, 0, 0, 0, TimeSpan.Zero),
            [
                ("Shriekwing", "BDGG", "US", new DateTimeOffset(2020, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Altimor the Huntsman", "BDGG", "US", new DateTimeOffset(2020, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Hungering Destroyer", "Lazarus Imperative", "US", new DateTimeOffset(2020, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Artificer Xy'Mox", "Complexity Limit", "US", new DateTimeOffset(2020, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Sun King's Salvation", "BDGG", "US", new DateTimeOffset(2020, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Lady Inerva Darkvein", "Complexity Limit", "US", new DateTimeOffset(2020, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("The Council of Blood", "Complexity Limit", "US", new DateTimeOffset(2020, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Sludgefist", "Complexity Limit", "US", new DateTimeOffset(2020, 12, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Stone Legion Generals", "Complexity Limit", "US", new DateTimeOffset(2020, 12, 21, 0, 0, 0, TimeSpan.Zero)),
                ("Sire Denathrius", "Complexity Limit", "US", new DateTimeOffset(2020, 12, 23, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Sanctum of Domination",
            new DateTimeOffset(2021, 7, 13, 0, 0, 0, TimeSpan.Zero),
            [
                ("The Tarragrue", "Soniqs Imperative", "US", new DateTimeOffset(2021, 7, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Eye of the Jailer", "Soniqs Imperative", "US", new DateTimeOffset(2021, 7, 13, 0, 0, 0, TimeSpan.Zero)),
                ("The Nine", "Soniqs Imperative", "US", new DateTimeOffset(2021, 7, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Remnant of Ner'zhul", "Complexity Limit", "US", new DateTimeOffset(2021, 7, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Soulrender Dormazain", "Complexity Limit", "US", new DateTimeOffset(2021, 7, 14, 0, 0, 0, TimeSpan.Zero)),
                ("Painsmith Raznal", "Echo", "EU", new DateTimeOffset(2021, 7, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Guardian of the First Ones", "Echo", "EU", new DateTimeOffset(2021, 7, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Fatescribe Roh-Kalo", "Complexity Limit", "US", new DateTimeOffset(2021, 7, 16, 0, 0, 0, TimeSpan.Zero)),
                ("Kel'Thuzad", "Echo", "EU", new DateTimeOffset(2021, 7, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Sylvanas Windrunner", "Echo", "EU", new DateTimeOffset(2021, 7, 20, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Sepulcher of the First Ones",
            new DateTimeOffset(2022, 3, 8, 0, 0, 0, TimeSpan.Zero),
            [
                ("Vigilant Guardian", "The Early Shift", "US", new DateTimeOffset(2022, 3, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Skolex, the Insatiable Ravener", "Liquid", "US", new DateTimeOffset(2022, 3, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Artificer Xy'mox", "Liquid", "US", new DateTimeOffset(2022, 3, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Dausegne, the Fallen Oracle", "Liquid", "US", new DateTimeOffset(2022, 3, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Prototype Pantheon", "Liquid", "US", new DateTimeOffset(2022, 3, 10, 0, 0, 0, TimeSpan.Zero)),
                ("Lihuvim, Principal Architect", "Liquid", "US", new DateTimeOffset(2022, 3, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Halondrus the Reclaimer", "Liquid", "US", new DateTimeOffset(2022, 3, 14, 0, 0, 0, TimeSpan.Zero)),
                ("Anduin Wrynn", "Liquid", "US", new DateTimeOffset(2022, 3, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Lords of Dread", "Echo", "EU", new DateTimeOffset(2022, 3, 19, 0, 0, 0, TimeSpan.Zero)),
                ("Rygelon", "Echo", "EU", new DateTimeOffset(2022, 3, 21, 0, 0, 0, TimeSpan.Zero)),
                ("The Jailer, Zovaal", "Echo", "EU", new DateTimeOffset(2022, 3, 26, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

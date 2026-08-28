using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 1: manuelles Seeding der aktuellen Season, damit das Dashboard gegen
/// echte Daten statt Mocks entwickelt werden kann. Nur für Development gedacht.
/// </summary>
public static class DbSeeder
{
    private static readonly string[] BossNames =
    [
        "Vexamus", "The Iron Choir", "Thane Drakksar", "Sable Weaver",
        "Grim Custodian", "Twin Sovereigns", "Ashen Court", "Voidbound Herald",
    ];

    // Eigener Expansion-Name (statt "The War Within"), damit dieser fiktive Demo-Datensatz
    // nicht mit der echten, quellenbasierten TWW-Historie aus TheWarWithinHistorySeeder
    // kollidiert — beide dürfen unabhängig voneinander existieren.
    private const string DemoExpansionName = "The War Within — Live Demo";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == DemoExpansionName)) return;

        var expansion = new Expansion { Id = Guid.NewGuid(), Name = DemoExpansionName, ReleaseDate = new DateOnly(2024, 8, 26) };
        var season = new Season
        {
            Id = Guid.NewGuid(),
            ExpansionId = expansion.Id,
            Number = 2,
            StartDate = new DateOnly(2025, 2, 25),
        };
        var raid = new Raid
        {
            Id = Guid.NewGuid(),
            SeasonId = season.Id,
            Name = "Nerub-ar Sanctum",
            BossCount = BossNames.Length,
            NormalOpenAt = new DateTimeOffset(2025, 2, 25, 0, 0, 0, TimeSpan.Zero),
            HeroicOpenAt = new DateTimeOffset(2025, 3, 4, 0, 0, 0, TimeSpan.Zero),
            MythicOpenAt = new DateTimeOffset(2025, 3, 4, 0, 0, 0, TimeSpan.Zero),
        };
        var bosses = BossNames
            .Select((name, i) => new Boss { Id = Guid.NewGuid(), RaidId = raid.Id, Name = name, Order = i })
            .ToList();

        // WarcraftLogs*-Felder bewusst leer gelassen: Ohne Mapping überspringt der
        // WarcraftLogsPollingService diese Gilden. Für echtes Live-Tracking die Felder
        // mit den WCL-Werten (Gildenname/Server-Slug/Region) und Boss.WarcraftLogsEncounterId
        // befüllen — siehe README.
        var guilds = new[]
        {
            new Guild { Id = Guid.NewGuid(), Name = "Liquid", Region = "EU", FoundedYear = 2016 },
            new Guild { Id = Guid.NewGuid(), Name = "Echo", Region = "EU", FoundedYear = 2013 },
            new Guild { Id = Guid.NewGuid(), Name = "Complexity Limit", Region = "NA", FoundedYear = 2018 },
        };

        db.Expansions.Add(expansion);
        db.Seasons.Add(season);
        db.Raids.Add(raid);
        db.Bosses.AddRange(bosses);
        db.Guilds.AddRange(guilds);

        // Liquid hat die ersten 6 Bosse erledigt.
        // Roster ist frei erfundene Demo-Besetzung (keine echten Personen) — zeigt im
        // Live-Demo-Modus, wie das Roster-Feature aussieht, wenn es echte Daten hätte.
        var demoRoster = new List<string>
        {
            "Naowh", "Maximum", "Fragnance", "Rhyolith", "Cayna", "Solanaceae",
            "Vaporlock", "Threep", "Xephyris", "Muffinus", "Kelaan", "Grennbean",
            "Vailea", "Anxxa", "Draikin", "Mercutiø", "Nightslayer", "Ellyra",
            "Fenrys", "Corrode",
        };
        var now = DateTimeOffset.UtcNow;
        for (var i = 0; i < 6; i++)
        {
            db.Kills.Add(new Kill
            {
                Id = Guid.NewGuid(),
                BossId = bosses[i].Id,
                GuildId = guilds[0].Id,
                Timestamp = now.AddHours(-(6 - i) * 3),
                PullCount = 40 + i * 15,
                Status = KillStatus.Confirmed,
                Roster = demoRoster,
            });
        }

        await db.SaveChangesAsync();
    }
}

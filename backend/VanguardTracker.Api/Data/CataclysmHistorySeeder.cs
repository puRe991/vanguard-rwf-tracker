using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Cataclysm-Historie — die letzte Ära vor verlässlicher
/// Warcraft-Logs-Abdeckung, daher noch manuell kuratiert statt via API. Gleiche
/// Herkunfts-Trennung wie <see cref="VanillaHistorySeeder"/>: Boss-Rosters sind
/// dokumentierter Spiel-Content, World-First-Ergebnisse (Gilde, Kill-Datum) stammen aus
/// "Cataclysm Raid History" — Method (https://www.method.gg/raid-history/cataclysm),
/// abgerufen 2026. Baradin Hold wird dort ohne Gilden-/Datumsangabe genannt und bleibt
/// entsprechend ohne Kill-Datensatz, offen für Community-Beiträge.
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

        var guilds = new Dictionary<string, Guild>();
        Guild GetOrAddGuild(string name, string region)
        {
            if (guilds.TryGetValue(name, out var existing)) return existing;
            var guild = new Guild { Id = Guid.NewGuid(), Name = name, Region = region };
            guilds[name] = guild;
            db.Guilds.Add(guild);
            return guild;
        }

        var paragon = GetOrAddGuild("Paragon", "EU");

        // Quelle nennt keine Gilde/kein Datum für Baradin Hold.
        AddRaidWithoutConfirmedKill(db, season.Id,
            raidName: "Baradin Hold",
            openAt: new DateTimeOffset(2010, 12, 7, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Argaloth"]);

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Blackwing Descent",
            openAt: new DateTimeOffset(2010, 12, 7, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Magmaw", "Omnotron Defense System", "Maloriak",
                "Atramedes", "Chimaeron", "Nefarian",
            ],
            worldFirstGuild: paragon,
            killAt: new DateTimeOffset(2011, 1, 9, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "The Bastion of Twilight",
            openAt: new DateTimeOffset(2010, 12, 7, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Halfus Wyrmbreaker", "Valiona and Theralion", "Ascendant Council",
                "Cho'gall", "Sinestra",
            ],
            worldFirstGuild: paragon,
            killAt: new DateTimeOffset(2011, 1, 20, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Throne of the Four Winds",
            openAt: new DateTimeOffset(2010, 12, 7, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Conclave of Wind", "Al'Akir"],
            worldFirstGuild: paragon,
            killAt: new DateTimeOffset(2011, 1, 24, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Firelands",
            openAt: new DateTimeOffset(2011, 6, 28, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Beth'tilac", "Lord Rhyolith", "Alysrazor", "Shannox",
                "Baleroc, the Gatekeeper", "Majordomo Staghelm", "Ragnaros",
            ],
            worldFirstGuild: paragon,
            killAt: new DateTimeOffset(2011, 7, 19, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Dragon Soul",
            openAt: new DateTimeOffset(2011, 11, 29, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Morchok", "Warlord Zon'ozz", "Yor'sahj the Unsleeping", "Hagara the Stormbinder",
                "Ultraxion", "Warmaster Blackhorn", "Spine of Deathwing", "Madness of Deathwing",
            ],
            worldFirstGuild: GetOrAddGuild("KIN Raiders", "KR"),
            killAt: new DateTimeOffset(2011, 12, 20, 0, 0, 0, TimeSpan.Zero));

        await db.SaveChangesAsync();
    }

    private static void AddRaidWithConfirmedKill(
        VanguardDbContext db,
        Guid seasonId,
        string raidName,
        DateTimeOffset openAt,
        string[] bossNames,
        Guild worldFirstGuild,
        DateTimeOffset killAt)
    {
        AddRaidWithBosses(db, seasonId, raidName, openAt, bossNames, out var bosses);
        var finalBoss = bosses[^1];

        db.Kills.Add(new Kill
        {
            Id = Guid.NewGuid(),
            BossId = finalBoss.Id,
            GuildId = worldFirstGuild.Id,
            Timestamp = killAt,
            PullCount = 0, // von der Quelle nicht beziffert
            SourceUrl = SourceUrl,
            Status = KillStatus.Confirmed,
        });
    }

    private static void AddRaidWithoutConfirmedKill(
        VanguardDbContext db,
        Guid seasonId,
        string raidName,
        DateTimeOffset openAt,
        string[] bossNames)
    {
        AddRaidWithBosses(db, seasonId, raidName, openAt, bossNames, out _);
    }

    private static void AddRaidWithBosses(
        VanguardDbContext db,
        Guid seasonId,
        string raidName,
        DateTimeOffset openAt,
        string[] bossNames,
        out List<Boss> bosses)
    {
        var raid = new Raid
        {
            Id = Guid.NewGuid(),
            SeasonId = seasonId,
            Name = raidName,
            BossCount = bossNames.Length,
            NormalOpenAt = openAt,
        };
        bosses = bossNames
            .Select((name, i) => new Boss { Id = Guid.NewGuid(), RaidId = raid.Id, Name = name, Order = i })
            .ToList();

        db.Raids.Add(raid);
        db.Bosses.AddRange(bosses);
    }
}

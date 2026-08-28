using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte The-Burning-Crusade-Historie. Gleiche Herkunfts-Trennung
/// wie <see cref="VanillaHistorySeeder"/>: Boss-Rosters sind dokumentierter Spiel-Content,
/// World-First-Ergebnisse (Gilde, Kill-Datum) stammen aus "The Burning Crusade Raid
/// History" — Method (https://www.method.gg/raid-history/the-burning-crusade),
/// abgerufen 2026, und werden nur übernommen, wenn die Quelle sie tatsächlich nennt.
/// Karazhan wird dort nur mit Release-Datum genannt ("Karazhan Cleared", ohne Gilde/
/// Datum) — bleibt entsprechend ohne Kill-Datensatz, offen für Community-Beiträge.
/// </summary>
public static class BurningCrusadeHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/the-burning-crusade";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "The Burning Crusade")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "The Burning Crusade",
            ReleaseDate = new DateOnly(2007, 1, 16),
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

        var nihilum = GetOrAddGuild("Nihilum", "EU");

        // Quelle nennt nur "Karazhan Cleared" mit Release-Datum, keine World-First-Gilde/Datum.
        AddRaidWithoutConfirmedKill(db, season.Id,
            raidName: "Karazhan",
            openAt: new DateTimeOffset(2007, 1, 28, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Attumen the Huntsman", "Moroes", "Maiden of Virtue", "The Opera Event",
                "The Curator", "Terestian Illhoof", "Shade of Aran", "Netherspite",
                "Chess Event", "Prince Malchezaar", "Nightbane",
            ]);

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Gruul's Lair",
            openAt: new DateTimeOffset(2007, 1, 16, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["High King Maulgar", "Gruul the Dragonkiller"],
            worldFirstGuild: nihilum,
            killAt: new DateTimeOffset(2007, 2, 3, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Magtheridon's Lair",
            openAt: new DateTimeOffset(2007, 1, 16, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Magtheridon"],
            worldFirstGuild: nihilum,
            killAt: new DateTimeOffset(2007, 2, 24, 0, 0, 0, TimeSpan.Zero));

        // Quelle vermerkt eine zunächst "gebuggte" Nihilum-Kill-Meldung, korrigiert um
        // ein "World First Legit"-Zitat von Method — als offizielles Ergebnis bleibt
        // aber durchgehend Nihilum am 29.03.2007 stehen, kein abweichendes Datum/Gilde.
        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Serpentshrine Cavern",
            openAt: new DateTimeOffset(2007, 1, 16, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Hydross the Unstable", "The Lurker Below", "Leotheras the Blind",
                "Fathom-Lord Karathress", "Morogrim Tidewalker", "Lady Vashj",
            ],
            worldFirstGuild: nihilum,
            killAt: new DateTimeOffset(2007, 3, 29, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Tempest Keep: The Eye",
            openAt: new DateTimeOffset(2007, 1, 16, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Al'ar", "Void Reaver", "High Astromancer Solarian", "Kael'thas Sunstrider"],
            worldFirstGuild: nihilum,
            killAt: new DateTimeOffset(2007, 5, 25, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Black Temple",
            openAt: new DateTimeOffset(2007, 5, 22, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "High Warlord Naj'entus", "Supremus", "Shade of Akama", "Teron Gorefiend",
                "Gurtogg Bloodboil", "Reliquary of Souls", "Mother Shahraz",
                "The Illidari Council", "Illidan Stormrage",
            ],
            worldFirstGuild: nihilum,
            killAt: new DateTimeOffset(2007, 6, 5, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Mount Hyjal",
            openAt: new DateTimeOffset(2007, 5, 22, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Rage Winterchill", "Anetheron", "Kaz'rogal", "Azgalor", "Archimonde"],
            worldFirstGuild: nihilum,
            killAt: new DateTimeOffset(2007, 6, 9, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Sunwell Plateau",
            openAt: new DateTimeOffset(2008, 3, 25, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Kalecgos", "Brutallus", "Felmyst", "Eredar Twins",
                "M'uru", "Kil'jaeden",
            ],
            worldFirstGuild: GetOrAddGuild("SK Gaming", "EU"),
            killAt: new DateTimeOffset(2008, 5, 25, 0, 0, 0, TimeSpan.Zero));

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

using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Wrath-of-the-Lich-King-Historie. Gleiche Herkunfts-Trennung
/// wie <see cref="VanillaHistorySeeder"/>: Boss-Rosters sind dokumentierter Spiel-Content,
/// World-First-Ergebnisse (Gilde, Kill-Datum) stammen aus "Wrath of the Lich King Raid
/// History" — Method (https://www.method.gg/raid-history/wrath-of-the-lich-king),
/// abgerufen 2026. Vault of Archavon wird dort ohne Boss-/Gilden-/Datumsangabe genannt
/// und bleibt entsprechend ohne Kill-Datensatz, offen für Community-Beiträge.
/// </summary>
public static class WrathHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/wrath-of-the-lich-king";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Wrath of the Lich King")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Wrath of the Lich King",
            ReleaseDate = new DateOnly(2008, 11, 13),
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

        var ensidia = GetOrAddGuild("Ensidia", "EU");
        var paragon = GetOrAddGuild("Paragon", "EU");

        // Quelle nennt keinen Boss/keine Gilde/kein Datum für Vault of Archavon.
        AddRaidWithoutConfirmedKill(db, season.Id,
            raidName: "Vault of Archavon",
            openAt: new DateTimeOffset(2008, 11, 13, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Archavon the Stone Watcher"]);

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Naxxramas",
            openAt: new DateTimeOffset(2008, 11, 13, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Anub'Rekhan", "Grand Widow Faerlina", "Maexxna",
                "Noth the Plaguebringer", "Heigan the Unclean", "Loatheb",
                "Instructor Razuvious", "Gothik the Harvester", "The Four Horsemen",
                "Patchwerk", "Grobbulus", "Gluth", "Thaddius",
                "Sapphiron", "Kel'Thuzad",
            ],
            worldFirstGuild: ensidia,
            killAt: new DateTimeOffset(2008, 11, 15, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "The Obsidian Sanctum",
            openAt: new DateTimeOffset(2008, 11, 13, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Sartharion"],
            worldFirstGuild: ensidia,
            killAt: new DateTimeOffset(2008, 11, 21, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "The Eye of Eternity",
            openAt: new DateTimeOffset(2008, 11, 13, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Malygos"],
            worldFirstGuild: ensidia,
            killAt: new DateTimeOffset(2008, 11, 15, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Ulduar",
            openAt: new DateTimeOffset(2009, 4, 14, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Flame Leviathan", "Ignis the Furnace Master", "Razorscale", "XT-002 Deconstructor",
                "Assembly of Iron", "Kologarn", "Auriaya", "Hodir", "Thorim", "Freya",
                "Mimiron", "General Vezax", "Yogg-Saron", "Algalon the Observer",
            ],
            worldFirstGuild: GetOrAddGuild("Stars", "TW"),
            killAt: new DateTimeOffset(2009, 7, 7, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Trial of the Grand Crusader",
            openAt: new DateTimeOffset(2009, 8, 4, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Northrend Beasts", "Lord Jaraxxus", "Faction Champions",
                "Val'kyr Twins", "Anub'arak",
            ],
            worldFirstGuild: paragon,
            killAt: new DateTimeOffset(2009, 9, 7, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Icecrown Citadel",
            openAt: new DateTimeOffset(2009, 12, 8, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Lord Marrowgar", "Lady Deathwhisper", "Gunship Battle", "Deathbringer Saurfang",
                "Festergut", "Rotface", "Professor Putricide", "Blood Prince Council",
                "Blood-Queen Lana'thel", "Valithria Dreamwalker", "Sindragosa", "The Lich King",
            ],
            worldFirstGuild: paragon,
            killAt: new DateTimeOffset(2010, 3, 26, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Ruby Sanctum",
            openAt: new DateTimeOffset(2010, 6, 30, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Halion"],
            worldFirstGuild: GetOrAddGuild("Premonition", "US"),
            killAt: new DateTimeOffset(2010, 6, 30, 0, 0, 0, TimeSpan.Zero));

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

using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Vanilla-Historie — es gibt für diese Ära keine
/// verlässliche API (Warcraft Logs deckt erst ab ca. Cataclysm zuverlässig ab),
/// daher stammen die Daten aus einer kuratierten Community-Quelle statt aus einer
/// API-Anbindung.
///
/// Zwei getrennte Herkünfte, nicht zu verwechseln:
/// - Boss-Rosters (Reihenfolge, Namen) sind öffentlich dokumentierter Spiel-Content,
///   keine strittigen/quellenpflichtigen Fakten.
/// - World-First-Ergebnisse (Gilde, Kill-Datum) stammen aus "Vanilla Raid History
///   of World Firsts in World of Warcraft" — Method (https://www.method.gg/raid-history),
///   abgerufen 2026, und werden nur übernommen, wenn die Quelle sie tatsächlich nennt.
///   Für Zul'Gurub und Ruins of Ahn'Qiraj liefert die Quelle keine Gilden-/Datumsangaben
///   — diese Raid-Tiers bleiben ohne Kill-Datensatz und warten auf Community-Beiträge
///   über den Submit-Kill-Workflow (Moderation vor Veröffentlichung), statt dass Daten
///   erfunden werden. Pull-Zahlen sind für die Vanilla-Ära generell nicht überliefert.
/// </summary>
public static class VanillaHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Classic")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Classic",
            ReleaseDate = new DateOnly(2004, 11, 23),
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

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Onyxia's Lair",
            openAt: new DateTimeOffset(2004, 11, 23, 0, 0, 0, TimeSpan.Zero),
            bossNames: ["Onyxia"],
            worldFirstGuild: GetOrAddGuild("Ruined", "US"),
            killAt: new DateTimeOffset(2005, 1, 30, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Molten Core",
            openAt: new DateTimeOffset(2004, 11, 23, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Lucifron", "Magmadar", "Gehennas", "Garr", "Baron Geddon",
                "Shazzrah", "Sulfuron Harbinger", "Golemagg the Incinerator",
                "Majordomo Executus", "Ragnaros",
            ],
            worldFirstGuild: GetOrAddGuild("Ascent", "US"),
            killAt: new DateTimeOffset(2005, 4, 25, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Blackwing Lair",
            openAt: new DateTimeOffset(2005, 7, 12, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Razorgore the Untamed", "Vaelastrasz the Corrupt", "Broodlord Lashlayer",
                "Firemaw", "Ebonroc", "Flamegor", "Chromaggus", "Nefarian",
            ],
            worldFirstGuild: GetOrAddGuild("Drama", "US"),
            killAt: new DateTimeOffset(2005, 9, 26, 0, 0, 0, TimeSpan.Zero));

        // Zul'Gurub: Quelle nennt nur das Release-Datum, keine World-First-Gilde/Kill-Datum.
        AddRaidWithoutConfirmedKill(db, season.Id,
            raidName: "Zul'Gurub",
            openAt: new DateTimeOffset(2005, 9, 13, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "High Priestess Jeklik", "High Priest Venoxis", "High Priestess Mar'li",
                "Bloodlord Mandokir", "Gahz'rilla", "Wushoolay", "Renataki", "Hazza'rah",
                "High Priest Thekal", "High Priestess Arlokk", "Jin'do the Hexxer",
                "Hakkar the Soulflayer",
            ]);

        // Ruins of Ahn'Qiraj (AQ20): in der Quelle nicht mit Datum/Gilde belegt.
        AddRaidWithoutConfirmedKill(db, season.Id,
            raidName: "Ruins of Ahn'Qiraj",
            openAt: new DateTimeOffset(2006, 1, 3, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Kurinnaxx", "General Rajaxx", "Moam", "Buru the Gorger",
                "Ayamiss the Hunter", "Ossirian the Unscarred",
            ]);

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Temple of Ahn'Qiraj",
            openAt: new DateTimeOffset(2006, 1, 3, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "The Prophet Skeram", "Lord Kri", "Princess Yauj", "Vem",
                "Battleguard Sartura", "Fankriss the Unyielding", "Viscidus",
                "Princess Huhuran", "Twin Emperors Vek'lor and Veknilash", "Ouro", "C'Thun",
            ],
            worldFirstGuild: GetOrAddGuild("Nihilum", "EU"),
            killAt: new DateTimeOffset(2006, 4, 25, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithConfirmedKill(db, season.Id,
            raidName: "Naxxramas",
            openAt: new DateTimeOffset(2006, 6, 20, 0, 0, 0, TimeSpan.Zero),
            bossNames:
            [
                "Anub'Rekhan", "Grand Widow Faerlina", "Maexxna",
                "Noth the Plaguebringer", "Heigan the Unclean", "Loatheb",
                "Instructor Razuvious", "Gothik the Harvester", "The Four Horsemen",
                "Patchwerk", "Grobbulus", "Gluth", "Thaddius",
                "Sapphiron", "Kel'Thuzad",
            ],
            worldFirstGuild: GetOrAddGuild("Nihilum", "EU"),
            killAt: new DateTimeOffset(2006, 9, 7, 0, 0, 0, TimeSpan.Zero));

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

using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Vanilla-Historie — es gibt für diese Ära keine
/// verlässliche API (Warcraft Logs deckt erst ab ca. Cataclysm zuverlässig ab),
/// daher stammen die Daten aus einer kuratierten Community-Quelle statt aus einer
/// API-Anbindung. Quelle: "Vanilla Raid History of World Firsts in World of
/// Warcraft" — Method (https://www.method.gg/raid-history), abgerufen 2026.
///
/// Absichtlich unvollständig: nur Fakten, die die Quelle tatsächlich nennt
/// (finaler Boss, World-First-Gilde, Kill-Datum), werden übernommen. Für
/// Zul'Gurub und Ruins of Ahn'Qiraj liefert die Quelle keine Gilden-/Datumsangaben
/// — diese Raid-Tiers werden ohne Kill-Datensatz angelegt und warten auf
/// Community-Beiträge über den Submit-Kill-Workflow (Moderation vor Veröffentlichung).
/// Ebenso wird pro Raid nur der belegte finale Boss erfasst, nicht die volle
/// Boss-Liste — die lässt sich später ergänzen, ohne bestehende Kills zu berühren.
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

        AddRaidWithKnownFinalBoss(db, season.Id,
            raidName: "Onyxia's Lair",
            openAt: new DateTimeOffset(2004, 11, 23, 0, 0, 0, TimeSpan.Zero),
            finalBossName: "Onyxia",
            worldFirstGuild: GetOrAddGuild("Ruined", "US"),
            killAt: new DateTimeOffset(2005, 1, 30, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithKnownFinalBoss(db, season.Id,
            raidName: "Molten Core",
            openAt: new DateTimeOffset(2004, 11, 23, 0, 0, 0, TimeSpan.Zero),
            finalBossName: "Ragnaros",
            worldFirstGuild: GetOrAddGuild("Ascent", "US"),
            killAt: new DateTimeOffset(2005, 4, 25, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithKnownFinalBoss(db, season.Id,
            raidName: "Blackwing Lair",
            openAt: new DateTimeOffset(2005, 7, 12, 0, 0, 0, TimeSpan.Zero),
            finalBossName: "Nefarian",
            worldFirstGuild: GetOrAddGuild("Drama", "US"),
            killAt: new DateTimeOffset(2005, 9, 26, 0, 0, 0, TimeSpan.Zero));

        // Zul'Gurub: Quelle nennt nur das Release-Datum, keine World-First-Gilde/Kill-Datum.
        AddRaidWithoutConfirmedKill(db, season.Id,
            raidName: "Zul'Gurub",
            openAt: new DateTimeOffset(2005, 9, 13, 0, 0, 0, TimeSpan.Zero));

        // Ruins of Ahn'Qiraj (AQ20): in der Quelle nicht mit Datum/Gilde belegt.
        AddRaidWithoutConfirmedKill(db, season.Id,
            raidName: "Ruins of Ahn'Qiraj",
            openAt: new DateTimeOffset(2006, 1, 3, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithKnownFinalBoss(db, season.Id,
            raidName: "Temple of Ahn'Qiraj",
            openAt: new DateTimeOffset(2006, 1, 3, 0, 0, 0, TimeSpan.Zero),
            finalBossName: "C'Thun",
            worldFirstGuild: GetOrAddGuild("Nihilum", "EU"),
            killAt: new DateTimeOffset(2006, 4, 25, 0, 0, 0, TimeSpan.Zero));

        AddRaidWithKnownFinalBoss(db, season.Id,
            raidName: "Naxxramas",
            openAt: new DateTimeOffset(2006, 6, 20, 0, 0, 0, TimeSpan.Zero),
            finalBossName: "Kel'Thuzad",
            worldFirstGuild: GetOrAddGuild("Nihilum", "EU"),
            killAt: new DateTimeOffset(2006, 9, 7, 0, 0, 0, TimeSpan.Zero));

        await db.SaveChangesAsync();
    }

    private static void AddRaidWithKnownFinalBoss(
        VanguardDbContext db,
        Guid seasonId,
        string raidName,
        DateTimeOffset openAt,
        string finalBossName,
        Guild worldFirstGuild,
        DateTimeOffset killAt)
    {
        var raid = new Raid
        {
            Id = Guid.NewGuid(),
            SeasonId = seasonId,
            Name = raidName,
            BossCount = 1, // nur der belegte finale Boss, siehe Klassen-Kommentar
            NormalOpenAt = openAt,
        };
        var finalBoss = new Boss { Id = Guid.NewGuid(), RaidId = raid.Id, Name = finalBossName, Order = 0 };

        db.Raids.Add(raid);
        db.Bosses.Add(finalBoss);
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
        DateTimeOffset openAt)
    {
        db.Raids.Add(new Raid
        {
            Id = Guid.NewGuid(),
            SeasonId = seasonId,
            Name = raidName,
            BossCount = 0,
            NormalOpenAt = openAt,
        });
    }
}

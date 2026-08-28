using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Gemeinsame Bausteine für die manuell kuratierten Historie-Seeder (Vanilla bis
/// Midnight). Zentral vor allem wegen <see cref="GetOrAddGuildAsync"/>: Gilden wie
/// Method, Paragon oder Liquid treten über mehrere Addons hinweg auf und müssen als
/// eine einzige Guild-Zeile wiederverwendet werden, sonst zerfällt das
/// Gilden-Profil-Feature (Track-Record über alle Seasons) in Duplikate.
/// </summary>
internal static class HistorySeederHelpers
{
    public static async Task<Guild> GetOrAddGuildAsync(VanguardDbContext db, string name, string region)
    {
        var existing = db.Guilds.Local.FirstOrDefault(g => g.Name == name)
            ?? await db.Guilds.FirstOrDefaultAsync(g => g.Name == name);
        if (existing is not null) return existing;

        var guild = new Guild { Id = Guid.NewGuid(), Name = name, Region = region };
        db.Guilds.Add(guild);
        return guild;
    }

    private static Raid AddRaidWithBosses(
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
        return raid;
    }

    /// <summary>
    /// Ein Kill-Datensatz pro Raid (nur der finale Boss) — für Ären, in denen die
    /// Quelle nur das Gesamt-Tier-Ergebnis nennt, nicht jeden Einzel-Boss.
    /// </summary>
    public static void AddRaidWithFinalBossKill(
        VanguardDbContext db,
        Guid seasonId,
        string raidName,
        DateTimeOffset openAt,
        string[] bossNames,
        Guild worldFirstGuild,
        DateTimeOffset killAt,
        string sourceUrl,
        int pullCount = 0)
    {
        AddRaidWithBosses(db, seasonId, raidName, openAt, bossNames, out var bosses);
        var finalBoss = bosses[^1];

        db.Kills.Add(new Kill
        {
            Id = Guid.NewGuid(),
            BossId = finalBoss.Id,
            GuildId = worldFirstGuild.Id,
            Timestamp = killAt,
            PullCount = pullCount, // von der Quelle meist nicht beziffert
            SourceUrl = sourceUrl,
            Status = KillStatus.Confirmed,
        });
    }

    /// <summary>
    /// Ein Kill-Datensatz pro Boss, jeweils eigene Weltrekord-Gilde/-Datum — für Ären,
    /// in denen die Quelle die World-First-Kette Boss für Boss dokumentiert. Liefert
    /// deutlich reichhaltigere Daten als <see cref="AddRaidWithFinalBossKill"/>, u. a.
    /// weil verschiedene Gilden verschiedene Bosse zuerst legen können.
    /// </summary>
    public static async Task AddRaidWithPerBossKillsAsync(
        VanguardDbContext db,
        Guid seasonId,
        string raidName,
        DateTimeOffset openAt,
        (string BossName, string GuildName, string Region, DateTimeOffset KillAt)[] bossKills,
        string sourceUrl)
    {
        var raid = new Raid
        {
            Id = Guid.NewGuid(),
            SeasonId = seasonId,
            Name = raidName,
            BossCount = bossKills.Length,
            NormalOpenAt = openAt,
        };
        db.Raids.Add(raid);

        var order = 0;
        foreach (var (bossName, guildName, region, killAt) in bossKills)
        {
            var boss = new Boss { Id = Guid.NewGuid(), RaidId = raid.Id, Name = bossName, Order = order++ };
            db.Bosses.Add(boss);

            var guild = await GetOrAddGuildAsync(db, guildName, region);
            db.Kills.Add(new Kill
            {
                Id = Guid.NewGuid(),
                BossId = boss.Id,
                GuildId = guild.Id,
                Timestamp = killAt,
                PullCount = 0,
                SourceUrl = sourceUrl,
                Status = KillStatus.Confirmed,
            });
        }
    }

    /// <summary>Nur das Boss-Roster, ohne jede Kill-Angabe — für Raids, zu denen die
    /// Quelle keine World-First-Gilde/kein Datum nennt (oder eine laufende Season).</summary>
    public static void AddRaidWithBossesOnly(
        VanguardDbContext db,
        Guid seasonId,
        string raidName,
        DateTimeOffset openAt,
        string[] bossNames)
    {
        AddRaidWithBosses(db, seasonId, raidName, openAt, bossNames, out _);
    }
}

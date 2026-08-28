using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Dragonflight-Historie. Quelle: "Dragonflight Raid
/// History" — Method (https://www.method.gg/raid-history/dragonflight), abgerufen 2026.
/// Boss-für-Boss-Weltrekorde wie bei <see cref="MistsOfPandariaHistorySeeder"/>.
/// </summary>
public static class DragonflightHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/dragonflight";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Dragonflight")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Dragonflight",
            ReleaseDate = new DateOnly(2022, 11, 28),
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
            "Vault of the Incarnates",
            new DateTimeOffset(2022, 12, 13, 0, 0, 0, TimeSpan.Zero),
            [
                ("Eranog", "Vesper", "US", new DateTimeOffset(2022, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Terros", "Vesper", "US", new DateTimeOffset(2022, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("The Primal Council", "Liquid", "US", new DateTimeOffset(2022, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Sennarth, the Cold Breath", "Liquid", "US", new DateTimeOffset(2022, 12, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Dathea, Ascended", "Liquid", "US", new DateTimeOffset(2022, 12, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Kurog Grimtotem", "Liquid", "US", new DateTimeOffset(2022, 12, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Broodkeeper Diurna", "Liquid", "US", new DateTimeOffset(2022, 12, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Raszageth the Storm-Eater", "Echo", "EU", new DateTimeOffset(2022, 12, 23, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Aberrus, the Shadowed Crucible",
            new DateTimeOffset(2023, 5, 9, 0, 0, 0, TimeSpan.Zero),
            [
                ("Kazzara, the Hellforged", "The Early Shift", "US", new DateTimeOffset(2023, 5, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Assault of the Zaqali", "Nerd Crew", "US", new DateTimeOffset(2023, 5, 10, 0, 0, 0, TimeSpan.Zero)),
                ("The Amalgamation Chamber", "Nerd Crew", "US", new DateTimeOffset(2023, 5, 10, 0, 0, 0, TimeSpan.Zero)),
                ("The Forgotten Experiments", "FatSharkYes", "EU", new DateTimeOffset(2023, 5, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Rashok, the Elder", "Liquid", "US", new DateTimeOffset(2023, 5, 12, 0, 0, 0, TimeSpan.Zero)),
                ("The Vigilant Steward, Zskarn", "Liquid", "US", new DateTimeOffset(2023, 5, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Magmorax", "Liquid", "US", new DateTimeOffset(2023, 5, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Echo of Neltharion", "Liquid", "US", new DateTimeOffset(2023, 5, 14, 0, 0, 0, TimeSpan.Zero)),
                ("Scalecommander Sarkareth", "Liquid", "US", new DateTimeOffset(2023, 5, 15, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Amirdrassil, the Dream's Hope",
            new DateTimeOffset(2023, 11, 14, 0, 0, 0, TimeSpan.Zero),
            [
                ("Gnarlroot", "End Myth", "US", new DateTimeOffset(2023, 11, 15, 0, 0, 0, TimeSpan.Zero)),
                ("Igira the Cruel", "Instant Dollars", "US", new DateTimeOffset(2023, 11, 16, 0, 0, 0, TimeSpan.Zero)),
                ("Volcoross", "Instant Dollars", "EU", new DateTimeOffset(2023, 11, 16, 0, 0, 0, TimeSpan.Zero)),
                ("Council of Dreams", "Instant Dollars", "US", new DateTimeOffset(2023, 11, 16, 0, 0, 0, TimeSpan.Zero)),
                ("Larodar, Keeper of the Flame", "Instant Dollars", "US", new DateTimeOffset(2023, 11, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Nymue, Weaver of the Cycle", "Instant Dollars", "US", new DateTimeOffset(2023, 11, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Smolderon", "Liquid", "US", new DateTimeOffset(2023, 11, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Tindral Sageswift, Seer of the Flame", "Liquid", "US", new DateTimeOffset(2023, 11, 22, 0, 0, 0, TimeSpan.Zero)),
                ("Fyrakk the Blazing", "Echo", "EU", new DateTimeOffset(2023, 11, 26, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

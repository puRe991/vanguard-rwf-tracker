using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3: manuell kuratierte Mists-of-Pandaria-Historie. Quelle: "Mists of Pandaria
/// Raid History" — Method (https://www.method.gg/raid-history/mists-of-pandaria),
/// abgerufen 2026. Anders als bei den früheren Ären dokumentiert die Quelle hier jeden
/// einzelnen Boss mit eigener Weltrekord-Gilde und -Datum (nicht nur den Tier-Clear) —
/// entsprechend reicher ist <see cref="HistorySeederHelpers.AddRaidWithPerBossKillsAsync"/>.
/// </summary>
public static class MistsOfPandariaHistorySeeder
{
    private const string SourceUrl = "https://www.method.gg/raid-history/mists-of-pandaria";

    public static async Task SeedAsync(VanguardDbContext db)
    {
        if (await db.Expansions.AnyAsync(e => e.Name == "Mists of Pandaria")) return;

        var expansion = new Expansion
        {
            Id = Guid.NewGuid(),
            Name = "Mists of Pandaria",
            ReleaseDate = new DateOnly(2012, 9, 25),
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
            "Mogu'shan Vaults",
            new DateTimeOffset(2012, 10, 2, 0, 0, 0, TimeSpan.Zero),
            [
                ("The Stone Guard", "Vodka", "US", new DateTimeOffset(2012, 10, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Feng the Accursed", "Blood Legion", "US", new DateTimeOffset(2012, 10, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Gara'jal the Spiritbinder", "Vodka", "US", new DateTimeOffset(2012, 10, 10, 0, 0, 0, TimeSpan.Zero)),
                ("The Spirit Kings", "Blood Legion", "US", new DateTimeOffset(2012, 10, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Elegon", "Blood Legion", "US", new DateTimeOffset(2012, 10, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Will of the Emperor", "Method", "EU", new DateTimeOffset(2012, 10, 12, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Heart of Fear",
            new DateTimeOffset(2012, 10, 30, 0, 0, 0, TimeSpan.Zero),
            [
                ("Imperial Vizier Zor'lok", "Method", "EU", new DateTimeOffset(2012, 11, 8, 0, 0, 0, TimeSpan.Zero)),
                ("Blade Lord Ta'yak", "DarkStorm", "EU", new DateTimeOffset(2012, 11, 8, 0, 0, 0, TimeSpan.Zero)),
                ("Garalon", "Method", "EU", new DateTimeOffset(2012, 11, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Wind Lord Mel'jarak", "Method", "EU", new DateTimeOffset(2012, 11, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Amber-Shaper Un'sok", "Blood Legion", "US", new DateTimeOffset(2012, 11, 9, 0, 0, 0, TimeSpan.Zero)),
                ("Empress Shek'zeer", "Blood Legion", "US", new DateTimeOffset(2012, 11, 11, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Terrace of Endless Spring",
            new DateTimeOffset(2012, 11, 13, 0, 0, 0, TimeSpan.Zero),
            [
                ("Protectors of the Endless", "Blood Legion", "US", new DateTimeOffset(2012, 11, 20, 0, 0, 0, TimeSpan.Zero)),
                ("Tsulong", "Blood Legion", "US", new DateTimeOffset(2012, 11, 21, 0, 0, 0, TimeSpan.Zero)),
                ("Lei Shi", "Blood Legion", "US", new DateTimeOffset(2012, 11, 21, 0, 0, 0, TimeSpan.Zero)),
                ("Sha of Fear", "Method", "EU", new DateTimeOffset(2012, 11, 25, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Throne of Thunder",
            new DateTimeOffset(2013, 3, 5, 0, 0, 0, TimeSpan.Zero),
            [
                ("Jin'rokh the Breaker", "Blood Legion", "US", new DateTimeOffset(2013, 3, 12, 0, 0, 0, TimeSpan.Zero)),
                ("Horridon", "Blood Legion", "US", new DateTimeOffset(2013, 3, 12, 0, 0, 0, TimeSpan.Zero)),
                ("Council of Elders", "Exodus", "US", new DateTimeOffset(2013, 3, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Tortos", "Exodus", "US", new DateTimeOffset(2013, 3, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Megaera", "Blood Legion", "US", new DateTimeOffset(2013, 3, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Ji-Kun", "Blood Legion", "US", new DateTimeOffset(2013, 3, 13, 0, 0, 0, TimeSpan.Zero)),
                ("Durumu the Forgotten", "Blood Legion", "US", new DateTimeOffset(2013, 3, 14, 0, 0, 0, TimeSpan.Zero)),
                ("Primordius", "Method", "EU", new DateTimeOffset(2013, 3, 14, 0, 0, 0, TimeSpan.Zero)),
                ("Dark Animus", "Method", "EU", new DateTimeOffset(2013, 3, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Iron Qon", "Method", "EU", new DateTimeOffset(2013, 3, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Twin Consorts", "Blood Legion", "US", new DateTimeOffset(2013, 3, 20, 0, 0, 0, TimeSpan.Zero)),
                // Ra-den: optionaler Geheim-Boss (Wochenquest), erst nach dem eigentlichen
                // Tier-Clear gelegt (11.04., also NACH Lei Shen). Absichtlich vor Lei Shen
                // einsortiert, damit Lei Shen — der von der Quelle genannte finale Boss/
                // Tier-Clear — der letzte Eintrag bleibt (so wird er als Tier-Ergebnis
                // gewertet, siehe HistoryController).
                ("Ra-den", "Method", "EU", new DateTimeOffset(2013, 4, 11, 0, 0, 0, TimeSpan.Zero)),
                ("Lei Shen", "Method", "EU", new DateTimeOffset(2013, 3, 26, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await HistorySeederHelpers.AddRaidWithPerBossKillsAsync(db, season.Id,
            "Siege of Orgrimmar",
            new DateTimeOffset(2013, 9, 10, 0, 0, 0, TimeSpan.Zero),
            [
                ("Immerseus", "Blood Legion", "US", new DateTimeOffset(2013, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Fallen Protectors", "Blood Legion", "US", new DateTimeOffset(2013, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Norushen", "Blood Legion", "US", new DateTimeOffset(2013, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Sha of Pride", "Blood Legion", "US", new DateTimeOffset(2013, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Galakras", "Blood Legion", "US", new DateTimeOffset(2013, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Iron Juggernaut", "Blood Legion", "US", new DateTimeOffset(2013, 9, 17, 0, 0, 0, TimeSpan.Zero)),
                ("Kor'kron Dark Shaman", "Blood Legion", "US", new DateTimeOffset(2013, 9, 18, 0, 0, 0, TimeSpan.Zero)),
                ("General Nazgrim", "Blood Legion", "US", new DateTimeOffset(2013, 9, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Malkorok", "Blood Legion", "US", new DateTimeOffset(2013, 9, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Spoils of Pandaria", "Blood Legion", "US", new DateTimeOffset(2013, 9, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Thok the Bloodthirsty", "Method", "EU", new DateTimeOffset(2013, 9, 18, 0, 0, 0, TimeSpan.Zero)),
                ("Siegecrafter Blackfuse", "Method", "EU", new DateTimeOffset(2013, 9, 21, 0, 0, 0, TimeSpan.Zero)),
                ("Paragons of the Klaxxi", "Method", "EU", new DateTimeOffset(2013, 9, 24, 0, 0, 0, TimeSpan.Zero)),
                ("Garrosh Hellscream", "Method", "EU", new DateTimeOffset(2013, 10, 1, 0, 0, 0, TimeSpan.Zero)),
            ],
            SourceUrl);

        await db.SaveChangesAsync();
    }
}

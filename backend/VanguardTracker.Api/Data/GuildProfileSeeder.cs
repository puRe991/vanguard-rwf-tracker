using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

/// <summary>
/// Phase 3b: Recap/Bio-Texte und Status für die immer wieder auftauchenden
/// "Hauptdarsteller"-Gilden der RWF-Geschichte. Läuft nach allen History-Seedern
/// (die Gilden existieren also bereits) und aktualisiert sie nur per Namens-Lookup —
/// legt selbst keine neuen Gilden an.
///
/// Recherchiert aus öffentlich verifizierbaren Quellen (u. a. Wikipedia/MOUZ-Artikel,
/// Blizzard Watch, mein-mmo.de, teamliquid.com, method.gg, PC Gamer), abgerufen 2026.
/// Bewusst nur für Gilden gesetzt, deren Geschichte sich belegen ließ — alle anderen
/// importierten Gilden bleiben ohne Bio/Status (GuildStatus.Unknown), statt Geschichte
/// zu erfinden.
/// </summary>
public static class GuildProfileSeeder
{
    public static async Task SeedAsync(VanguardDbContext db)
    {
        // Marker: Wenn die bekannteste Gilde bereits eine Bio hat, war der Seeder schon da.
        var alreadySeeded = await db.Guilds.AnyAsync(g => g.Name == "Method" && g.Bio != null);
        if (alreadySeeded) return;

        await UpdateAsync(db, "Nihilum",
            bio: "Gegründet 2005 auf Magtheridon-EU, war Nihilum die dominante Gilde der " +
                 "späten Vanilla- und der gesamten TBC-Ära: acht World-Firsts in Folge, vom " +
                 "ersten Kill C'Thuns in Ahn'Qiraj 2006 bis zu Illidan im Black Temple 2007. " +
                 "Die Serie endete erst, als SK Gaming (EU) 2008 Kil'jaeden in Sunwell Plateau " +
                 "zuerst legte. Kurz danach zerbrach die Partnerschaft mit Organisation " +
                 "mousesports; im November 2008 schlossen sich Nihilums Raid-Kern und die " +
                 "PvE-Spieler von SK Gaming zur neuen Gilde Ensidia zusammen — Nihilum als " +
                 "eigenständige Gilde existierte damit nicht mehr.",
            status: GuildStatus.Disbanded,
            disbandedYear: 2008);

        await UpdateAsync(db, "Ensidia",
            bio: "Aus dem Zusammenschluss von Nihilum und der PvE-Mannschaft von SK Gaming " +
                 "entstanden, wurde der Name Ensidia am 25. November 2008 offiziell bekannt " +
                 "gegeben — passend dazu launchte Wrath of the Lich King wenige Tage zuvor. " +
                 "Ensidia dominierte den Start der Erweiterung (u. a. Naxxramas, Obsidian " +
                 "Sanctum, Eye of Eternity als Erste bezwungen) und sammelte über die Jahre " +
                 "mehr World-First-Kills als jede andere Gilde ihrer Zeit zusammen. Am " +
                 "13. Januar 2012 löste sich die Gilde offiziell auf.",
            status: GuildStatus.Disbanded,
            disbandedYear: 2012);

        await UpdateAsync(db, "Paragon",
            bio: "Die finnische Gilde Paragon war über Jahre die Nummer zwei der Szene direkt " +
                 "hinter Method, mit dem beide sich den Spitzenplatz mehrfach gegenseitig " +
                 "abnahmen. Höhepunkte waren der World-First-Kill des Lich King in Icecrown " +
                 "Citadel (März 2010) sowie zahlreiche Erstkills durch Cataclysm und Mists of " +
                 "Pandaria, darunter Heroic Garrosh Hellscream. In Warlords of Draenor gelang " +
                 "noch der Weltrekord auf Mythic Imperator Mar'gok in Highmaul, bevor Paragon " +
                 "im Februar 2016 die Auflösung bekanntgab — der Gilde fehlte schlicht ein " +
                 "ausreichend großer Pool konkurrenzfähiger finnischer Spieler, und eine " +
                 "internationale Öffnung war nicht der Plan.",
            status: GuildStatus.Disbanded,
            disbandedYear: 2016);

        await UpdateAsync(db, "Method",
            bio: "Method wurde 2005 von Sco als Horde-Raidgilde gegründet und ist bis heute " +
                 "die Gilde mit den meisten Endboss-World-Firsts der WoW-Geschichte. Im Juni " +
                 "2020 geriet die Organisation durch Vorwürfe sexuellen Fehlverhaltens gegen " +
                 "ein Mitglied sowie gegen Co-CEO Sascha Steffens in eine schwere Krise — " +
                 "Sponsoren wie MSI und Corsair zogen sich zurück, und der Großteil des " +
                 "damaligen Raid-Kaders verließ die Gilde, um kurz darauf die neue " +
                 "Organisation Echo zu gründen. Method blieb unter Sco bestehen, baute einen " +
                 "neuen Kader auf und raidet bis heute (Stand 2026) weiter um Weltrekorde mit.",
            status: GuildStatus.Active,
            foundedYear: 2005,
            twitchUrl: "https://www.twitch.tv/method",
            websiteUrl: "https://www.method.gg");

        await UpdateAsync(db, "Echo",
            bio: "Echo entstand im Juli 2020 aus dem größten Teil des ehemaligen " +
                 "Method-Raid-Kaders, kurz nachdem die Method-Organisation durch einen " +
                 "Missbrauchsskandal auseinandergebrochen war. Seit Shadowlands zählt Echo " +
                 "durchgehend zur absoluten Weltspitze und hat unter anderem Sepulcher of the " +
                 "First Ones, Vault of the Incarnates und Amirdrassil, the Dream's Hope als " +
                 "Erste bezwungen.",
            status: GuildStatus.Active,
            foundedYear: 2020,
            twitchUrl: "https://www.twitch.tv/echo_esports",
            twitterUrl: "https://twitter.com/EchoGuild",
            websiteUrl: "https://www.echoesports.gg");

        await UpdateAsync(db, "Blood Legion",
            bio: "Blood Legion war seit dem Launch von World of Warcraft eine feste Größe an " +
                 "der Weltspitze und Gegenstand der Dokumentation \"Race to World First\" " +
                 "(2013). Im März 2015 zog sich die Gilde vom Spitzen-Wettkampf zurück — laut " +
                 "Raidleiter Riggnaros forderten die realen Verpflichtungen der über die Jahre " +
                 "gealterten Stammspieler und der enorme Zeitaufwand ihren Tribut. Die Gilde " +
                 "löste sich nicht komplett auf, raidet seither aber nicht mehr auf " +
                 "World-First-Niveau.",
            status: GuildStatus.Retired,
            disbandedYear: 2015);

        // Limit -> Complexity Limit -> Liquid Guild: eine durchgehende Organisation unter drei
        // Namen. Da unser Datenmodell Gilden pro verwendetem Namen abbildet (jede Ära nutzte
        // den damals aktuellen Namen für World-First-Meldungen), bekommt jeder Name eine eigene
        // Bio, die die Kontinuität erklärt, statt die drei Zeilen künstlich zusammenzuführen.
        await UpdateAsync(db, "Limit",
            bio: "Limit wurde Anfang 2015 aus Kernspielern mehrerer nordamerikanischer " +
                 "Top-Gilden gegründet und zählte von Legion bis Battle for Azeroth zur " +
                 "Weltspitze. Im Oktober 2019 ging die Gilde eine Partnerschaft mit der " +
                 "Organisation Complexity ein und trat fortan als Complexity Limit an.",
            status: GuildStatus.Disbanded,
            disbandedYear: 2019);

        await UpdateAsync(db, "Complexity Limit",
            bio: "Aus der im Oktober 2019 geschlossenen Partnerschaft von Limit mit der " +
                 "Organisation Complexity hervorgegangen, erreichte Complexity Limit im " +
                 "Februar 2020 mit Ny'alotha, the Waking City erstmals den Rang der weltbesten " +
                 "Gilde. Im Januar 2022 wurde die Gilde von Team Liquid übernommen und trat " +
                 "fortan als Liquid Guild an — Gildenleiter Max \"Maximum\" Smith erhielt dabei " +
                 "eine Beteiligung an Team Liquid.",
            status: GuildStatus.Disbanded,
            disbandedYear: 2022);

        await UpdateAsync(db, "Liquid",
            bio: "Liquid Guild ist die direkte Fortsetzung von Complexity Limit (zuvor Limit, " +
                 "gegründet 2015): Im Januar 2022 übernahm die Esports-Organisation Team " +
                 "Liquid die Gilde und gliederte sie als eigene MMO-Sparte ein. Seit " +
                 "Sepulcher of the First Ones zählt Liquid durchgehend zur Weltspitze und hat " +
                 "unter anderem Nerub'ar Palace, Liberation of Undermine, Manaforge Omega und " +
                 "die ersten beiden Midnight-Raids als Erste bezwungen.",
            status: GuildStatus.Active,
            foundedYear: 2015,
            twitchUrl: "https://www.twitch.tv/teamliquid",
            youtubeUrl: "https://www.youtube.com/@TeamLiquidMMO",
            twitterUrl: "https://x.com/LiquidGuild",
            websiteUrl: "https://teamliquid.com/games/wow");

        await db.SaveChangesAsync();
    }

    private static async Task UpdateAsync(
        VanguardDbContext db,
        string name,
        string bio,
        GuildStatus status,
        int? disbandedYear = null,
        int? foundedYear = null,
        string? twitchUrl = null,
        string? youtubeUrl = null,
        string? twitterUrl = null,
        string? websiteUrl = null)
    {
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Name == name);
        if (guild is null) return; // Gilde nicht importiert — nichts zu tun.

        guild.Bio = bio;
        guild.Status = status;
        guild.DisbandedYear = disbandedYear;
        if (foundedYear.HasValue) guild.FoundedYear = foundedYear;
        guild.TwitchUrl = twitchUrl;
        guild.YoutubeUrl = youtubeUrl;
        guild.TwitterUrl = twitterUrl;
        guild.WebsiteUrl = websiteUrl;
    }
}

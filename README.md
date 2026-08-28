# Vanguard — Race to World First Community-Tracker

Web-Plattform für die World-of-Warcraft-"Race to World First": Live-Dashboard
der aktuellen Race, Historie seit Classic und Community-kuratierte Kills für
Zeiträume ohne verlässliche API-Abdeckung.

## Struktur

- `backend/` — ASP.NET Core 8 Web-API (C#, EF Core, PostgreSQL, SignalR)
- `frontend/` — React (Vite + TypeScript), TanStack Query, Tailwind CSS

## Backend starten

Voraussetzung: .NET 8 SDK, PostgreSQL.

```bash
cd backend/VanguardTracker.Api
cp appsettings.json appsettings.Development.local.json  # Connection-String/JWT-Key anpassen
dotnet restore
dotnet ef migrations add InitialCreate   # einmalig, benötigt dotnet-ef (dotnet tool install --global dotnet-ef)
dotnet run
```

Die API läuft dann unter `https://localhost:5xxx`, Swagger unter `/swagger`.
Im Development-Modus werden Migrationen automatisch angewendet und die
aktuelle Season wird mit Beispieldaten geseedet (`Data/DbSeeder.cs`).

## Frontend starten

Voraussetzung: Node 20+.

```bash
cd frontend
npm install
cp .env.example .env   # VITE_USE_MOCKS=true lässt das Dashboard ohne Backend laufen
npm run dev
```

Solange `VITE_USE_MOCKS=true` gesetzt ist, arbeitet das Frontend mit
Mock-Daten (`src/mocks/data.ts`) — nützlich für die reine UI-Entwicklung.
Sobald das Backend läuft, `VITE_USE_MOCKS=false` setzen und `VITE_API_BASE_URL`
auf die Backend-URL zeigen lassen.

## Warcraft-Logs-Live-Tracking (Phase 2)

`Services/WarcraftLogsPollingService.cs` pollt periodisch die
[Warcraft Logs API v2](https://www.warcraftlogs.com/api/docs) für die aktuelle
Mythic-Race, erkennt neue Pulls/Kills und pusht sie über den `RaceHub`
(`TickerEvent` für den Live-Ticker, `RaceUpdated` als Signal für das Frontend,
die Rangliste neu zu laden). Bestätigte Kills landen automatisch als
`Kill`-Datensätze mit `Status = Confirmed`.

**Einrichtung:**

1. Client-App unter <https://www.warcraftlogs.com/api/clients/> anlegen
   (V2-Client, Client-Credentials-Flow) und `WarcraftLogs:ClientId` /
   `WarcraftLogs:ClientSecret` in `appsettings.Development.local.json` setzen.
   Ohne Zugangsdaten überspringt der Job das Polling (siehe Log-Ausgabe).
2. Pro Gilde, die live getrackt werden soll, `Guild.WarcraftLogsGuildName`,
   `WarcraftLogsServerSlug` und `WarcraftLogsServerRegion` setzen (wie von der
   WCL-API erwartet, z. B. `serverSlug: "tarren-mill"`, `serverRegion: "EU"`).
3. Pro Boss `Boss.WarcraftLogsEncounterId` auf die WCL-`encounterID` setzen.
4. Gilden/Bosse ohne Mapping werden vom Poller stillschweigend übersprungen —
   praktisch für Community-kuratierte Inhalte vor Cataclysm, die nie eine
   API-Quelle haben werden.

Der Job wertet nur Reports aus, deren letzter Fight länger als
`WarcraftLogs:ReportFinalizationGraceMinutes` zurückliegt, um nicht auf
unvollständig hochgeladene Live-Reports hereinzufallen.

## Historie-Import (Phase 3)

Für die Vanilla-Ära (Classic, Season 1) gibt es keine API-Abdeckung, daher ist
`Data/VanillaHistorySeeder.cs` eine manuell kuratierte Übernahme aus
["Vanilla Raid History of World Firsts in World of Warcraft"](https://www.method.gg/raid-history)
(Method), automatisch eingespielt im Development-Modus zusammen mit
`DbSeeder`. Übernommen wird nur, was die Quelle tatsächlich belegt:

| Raid | Finaler Boss | World-First-Gilde | Kill-Datum |
|---|---|---|---|
| Onyxia's Lair | Onyxia | Ruined (US) | 30.01.2005 |
| Molten Core | Ragnaros | Ascent (US) | 25.04.2005 |
| Blackwing Lair | Nefarian | Drama (US) | 26.09.2005 |
| Zul'Gurub | — | — (nicht belegt) | — |
| Ruins of Ahn'Qiraj | — | — (nicht belegt) | — |
| Temple of Ahn'Qiraj | C'Thun | Nihilum (EU) | 25.04.2006 |
| Naxxramas | Kel'Thuzad | Nihilum (EU) | 07.09.2006 |

`Data/BurningCrusadeHistorySeeder.cs` führt dieselbe Kuration für The Burning
Crusade fort, Quelle
[method.gg/raid-history/the-burning-crusade](https://www.method.gg/raid-history/the-burning-crusade):

| Raid | Finaler Boss | World-First-Gilde | Kill-Datum |
|---|---|---|---|
| Karazhan | — | — (nicht belegt) | — |
| Gruul's Lair | Gruul the Dragonkiller | Nihilum (EU) | 03.02.2007 |
| Magtheridon's Lair | Magtheridon | Nihilum (EU) | 24.02.2007 |
| Serpentshrine Cavern¹ | Lady Vashj | Nihilum (EU) | 29.03.2007 |
| Tempest Keep: The Eye | Kael'thas Sunstrider | Nihilum (EU) | 25.05.2007 |
| Black Temple | Illidan Stormrage | Nihilum (EU) | 05.06.2007 |
| Mount Hyjal | Archimonde | Nihilum (EU) | 09.06.2007 |
| Sunwell Plateau | Kil'jaeden | SK Gaming (EU) | 25.05.2008 |

¹ Die Quelle erwähnt eine zunächst "gebuggte" Kill-Meldung von Nihilum, korrigiert
durch ein "World First Legit"-Zitat von Method — als Ergebnis bleibt aber
durchgehend Nihilum am 29.03.2007 stehen, ohne abweichendes Datum oder Gilde.

`Data/WrathHistorySeeder.cs` führt die Kuration für Wrath of the Lich King fort,
Quelle [method.gg/raid-history/wrath-of-the-lich-king](https://www.method.gg/raid-history/wrath-of-the-lich-king):

| Raid | Finaler Boss | World-First-Gilde | Kill-Datum |
|---|---|---|---|
| Vault of Archavon | — | — (nicht belegt) | — |
| Naxxramas | Kel'Thuzad | Ensidia (EU) | 15.11.2008 |
| The Obsidian Sanctum | Sartharion | Ensidia (EU) | 21.11.2008 |
| The Eye of Eternity | Malygos | Ensidia (EU) | 15.11.2008 |
| Ulduar | Yogg-Saron | Stars (TW) | 07.07.2009 |
| Trial of the Grand Crusader | Anub'arak | Paragon (EU) | 07.09.2009 |
| Icecrown Citadel | The Lich King | Paragon (EU) | 26.03.2010 |
| Ruby Sanctum | Halion | Premonition (US) | 30.06.2010 |

`Data/CataclysmHistorySeeder.cs` schließt die manuell kuratierte Ära ab (letzte
Season ohne verlässliche Warcraft-Logs-Abdeckung), Quelle
[method.gg/raid-history/cataclysm](https://www.method.gg/raid-history/cataclysm):

| Raid | Finaler Boss | World-First-Gilde | Kill-Datum |
|---|---|---|---|
| Baradin Hold | — | — (nicht belegt) | — |
| Blackwing Descent | Nefarian | Paragon (EU) | 09.01.2011 |
| The Bastion of Twilight | Sinestra | Paragon (EU) | 20.01.2011 |
| Throne of the Four Winds | Al'Akir | Paragon (EU) | 24.01.2011 |
| Firelands | Ragnaros | Paragon (EU) | 19.07.2011 |
| Dragon Soul | Madness of Deathwing | KIN Raiders (KR) | 20.12.2011 |

Jeder importierte Kill trägt die Quelle als `SourceUrl` (Beleg-Pflicht laut
Datenmodell). Raids ohne belegte World-First-Angabe (Zul'Gurub, Ruins of
Ahn'Qiraj, Karazhan, Vault of Archavon, Baradin Hold) werden bewusst ohne
Kill-Datensatz angelegt, statt Daten zu erfinden — die History-Seite verlinkt
dafür direkt auf den Community-Beitrags-Workflow (`/submit`) und zeigt
trotzdem die vollständige, dokumentierte Boss-Liste jedes Raids an. Pull-Zahlen
sind für diese Ären generell nicht überliefert und stehen daher auf `0`; die
History-UI blendet `0 Pulls` aus statt sie als Fakt darzustellen.

Ab Mists of Pandaria dokumentiert Method jeden Boss einzeln mit eigener
Weltrekord-Gilde/-Datum statt nur das Tier-Gesamtergebnis. Dafür gibt es
`HistorySeederHelpers.AddRaidWithPerBossKillsAsync` — jeder Boss bekommt seinen
eigenen `Kill`-Datensatz (verschiedene Gilden können verschiedene Bosse zuerst
legen), der letzte Eintrag pro Raid markiert weiterhin den Tier-Clear. Optionale
Geheim-Bosse (Algalon in Ulduar, Ra-den in Throne of Thunder), die nach dem
regulären Tier-Clear gelegt wurden, sind bewusst nicht als letzter Eintrag
angelegt, damit sie nicht fälschlich als Tier-Ergebnis gezählt werden.

Kuratiert bis einschließlich Midnight (aktuelles Addon, Stand des Method-Imports):
`Data/MistsOfPandariaHistorySeeder.cs`, `WarlordsHistorySeeder.cs`,
`LegionHistorySeeder.cs`, `BattleForAzerothHistorySeeder.cs`,
`ShadowlandsHistorySeeder.cs`, `DragonflightHistorySeeder.cs`,
`TheWarWithinHistorySeeder.cs`, `MidnightHistorySeeder.cs` — je Quelle
`method.gg/raid-history/<addon-slug>`. Tier-Clears (finaler Boss/Gilde/Datum):

| Addon | Letzter Raid | Finaler Boss | World-First-Gilde | Kill-Datum |
|---|---|---|---|---|
| Mists of Pandaria | Siege of Orgrimmar | Garrosh Hellscream | Method (EU) | 01.10.2013 |
| Warlords of Draenor | Hellfire Citadel | Archimonde | Method (EU) | 16.07.2015 |
| Legion | Antorus, the Burning Throne | Argus the Unmaker | Method (EU) | 13.12.2017 |
| Battle for Azeroth | Ny'alotha, the Waking City | N'Zoth the Corruptor | Complexity Limit (US) | 06.02.2020 |
| Shadowlands | Sepulcher of the First Ones | The Jailer, Zovaal | Echo (EU) | 26.03.2022 |
| Dragonflight | Amirdrassil, the Dream's Hope | Fyrakk the Blazing | Echo (EU) | 26.11.2023 |
| The War Within | Manaforge Omega | Dimensius, the All-Devouring | Liquid (US) | 24.08.2025 |
| Midnight (Season 1) | March on Quel'Danas | Midnight Falls (L'ura) | Liquid (US) | 06.04.2026 |

Midnight Season 2 (The Venomous Abyss, seit 18.08.2026) läuft laut Quelle noch
("Status: In Progress") — Boss-Roster ist bekannt, World-Firsts noch nicht,
daher ohne Kill-Datensätze angelegt statt Ergebnisse zu erfinden.

Die Guild-Zuordnung läuft über `HistorySeederHelpers.GetOrAddGuildAsync`, das
zuerst bereits gespeicherte/getrackte Gilden per Name wiederverwendet — Gilden
wie Method, Paragon, Liquid oder Echo treten über mehrere Addons hinweg auf und
bekommen so korrekt eine einzige Guild-Zeile statt Duplikaten pro Seeder
(wichtig für das Gilden-Profil-Feature).

Damit ist die manuell kuratierte Historie durchgängig bis zum aktuellen Addon
importiert. Ab Cataclysm/Mists of Pandaria (verlässliche Warcraft-Logs-Abdeckung)
könnte ein späterer Import stattdessen `WarcraftLogsClient` wiederverwenden, um
Guild-Reports vergangener Tiers systematisch statt Boss-für-Boss aus einer
Zweitquelle abzufragen.

## Gilden-Profile: Recap & Kill-Historie

`Data/GuildProfileSeeder.cs` ergänzt einzelne, wiederkehrende Gilden um einen
Recap-Text (Entstehung, Höhepunkte, Auflösung/Nachfolge), einen Lifecycle-Status
(`Active`/`Disbanded`/`Retired`/`Unknown`) und Social-Links (Twitch/YouTube/
Twitter/Website) — recherchiert aus öffentlich verifizierbaren Quellen (u. a.
Wikipedia, Blizzard Watch, mein-mmo.de, teamliquid.com, method.gg,
echoesports.gg), abgerufen 2026: Nihilum, Ensidia, Paragon, Method, Echo,
Blood Legion sowie die Limit → Complexity Limit → Liquid-Linie. Alle anderen
importierten Gilden bleiben bewusst ohne Bio (`Status = Unknown`) statt
Geschichte zu erfinden — das Frontend zeigt für sie einfach keinen
Recap-Block.

`GET /api/guilds/{id}/profile` liefert Bio/Status/Links plus die komplette
Kill-Historie der Gilde über alle Seasons hinweg, gruppiert nach Addon im
Frontend (`GuildProfile.tsx`). Jeder Kill trägt seinen `SourceUrl` als
"Beleg/Video"-Link — bei live getrackten Kills der Warcraft-Logs-Fight-Link,
bei kuratierter Historie der jeweilige Method-Quellenlink. Eine echte
Video-Bibliothek pro Kill oder eingebettete Twitch-Streams gibt es bewusst
nicht: Ohne YouTube-/Twitch-API-Anbindung wäre das nur weitere unbelegte
Daten — die Social-Links verweisen stattdessen auf die offiziellen Kanäle,
auf denen sich die echten VODs finden lassen.

## Design-System

Siehe `frontend/src/index.css` (`@theme`-Block) für die Farb- und
Typografie-Tokens des "Vanguard"-Designs (Obsidian/Türkis/Gold/Ember,
Cinzel/Barlow Condensed/Inter/JetBrains Mono).

## Phasenplan

1. **MVP** (erledigt): EF-Core-Schema + manuelles Seeding, React-Dashboard
   gegen Mock- bzw. geseedete Daten, kein Live-Polling.
2. **Live-Tracking** (erledigt): SignalR-Hub + Warcraft-Logs-Polling für
   automatische Kill-/Pull-Erkennung der aktuellen Mythic-Race, siehe
   „Warcraft-Logs-Live-Tracking" unten.
3. **Historie-Import** (erledigt): manuell kuratiert von Classic bis Midnight
   (`Data/*HistorySeeder.cs`), siehe „Historie-Import" unten.
4. Community-Beitrags-Workflow + Moderation (JWT-Auth ist im Backend
   bereits verdrahtet, `POST /api/kills/submit` erfordert `[Authorize]`).
5. Benachrichtigungen/Discord-Integration.

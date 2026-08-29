# Vanguard — Race to World First Community-Tracker

Web-Plattform für die World-of-Warcraft-"Race to World First": Live-Dashboard
der aktuellen Race, Historie seit Classic und Community-kuratierte Kills für
Zeiträume ohne verlässliche API-Abdeckung.

## Struktur

- `backend/` — ASP.NET Core 8 Web-API (C#, EF Core, PostgreSQL, SignalR)
- `frontend/` — React (Vite + TypeScript), TanStack Query, Tailwind CSS

## Schnellstart

```powershell
# Windows (PowerShell oder Doppelklick auf start.cmd)
.\start.ps1
```

```bash
# Linux / macOS
./start.sh
```

Das Startskript prueft die Werkzeuge (.NET 8 SDK, Node 20+, npm), stellt die
Backend-Pakete wieder her, installiert und **verifiziert** die
Frontend-Abhaengigkeiten, legt bei Bedarf `frontend/.env` aus `.env.example` an
und startet Backend (`http://localhost:5000`) und Frontend
(`http://localhost:5173`).

Nuetzliche Schalter: `-SkipBackend` / `--skip-backend`, `-SkipFrontend` /
`--skip-frontend`, `-InstallOnly` / `--install-only`.

### Frontend-Installation reparieren

Die Frontend-Toolchain (vite/rolldown, Tailwind Oxide, lightningcss) laedt
native Binaries, die als plattformspezifische `optionalDependencies`
ausgeliefert werden. npm ueberspringt diese Pakete unter bestimmten Umstaenden
([npm/cli#4828](https://github.com/npm/cli/issues/4828)) — vor allem, wenn ueber
einen bereits vorhandenen `node_modules`-Baum installiert wird oder
`package-lock.json` auf einer anderen Plattform erzeugt wurde. Symptom:

```
Cannot find native binding. npm has a bug related to optional dependencies ...
```

`frontend/scripts/setup.mjs` behandelt genau das:

```bash
cd frontend
npm run setup
```

Das Skript importiert die tatsaechlich benoetigten Pakete (statt nur zu pruefen,
ob `node_modules` existiert) und repariert bei Bedarf in **hoechstens drei**
Stufen — `npm ci`, dann `node_modules` weg + `npm ci`, dann zusaetzlich
`package-lock.json` weg + `npm install`. Danach bricht es mit einer konkreten
Fehlermeldung ab, statt bei jedem Start erneut alles neu zu installieren.

Wichtig: `package-lock.json` ist eingecheckt und enthaelt die nativen Binaries
**aller** Plattformen. Deshalb ist `npm ci` die richtige Reparatur —
`package-lock.json` zu loeschen ist die *letzte*, nicht die erste Massnahme.

## Backend starten

Voraussetzung: .NET 8 SDK, PostgreSQL.

```bash
cd backend/VanguardTracker.Api
cp appsettings.json appsettings.Development.local.json  # Connection-String/JWT-Key anpassen
dotnet restore
dotnet run
```

`appsettings.Development.local.json` wird von `Program.cs` als zusätzliche
Konfigurationsquelle nach den eingecheckten `appsettings.*.json` geladen und
gewinnt damit gegen deren Platzhalter (`Password=changeme`,
`CHANGE_ME_TO_A_LONG_RANDOM_SECRET…`). Die Datei ist per `.gitignore`
ausgeschlossen. `dotnet run` nimmt das Profil `http` aus
`Properties/launchSettings.json` und setzt darüber
`ASPNETCORE_ENVIRONMENT=Development` — ohne das liefe die App in `Production`,
also ohne Migration, Seeder und Swagger.

Die `InitialCreate`-Migration liegt bereits unter `Migrations/` im Repo — im
Development-Modus wendet `Program.cs` sie beim Start automatisch an
(`db.Database.MigrateAsync()`) und lässt danach alle Seeder durchlaufen
(aktuelle Demo-Season + komplette Historie Classic→Midnight). Nach
Modelländerungen eine neue Migration erzeugen mit
`dotnet-ef migrations add <Name>` (Tool via
`dotnet tool install --global dotnet-ef`).

Die API läuft dann unter `http://localhost:5000` — dieselbe Adresse, auf die
`frontend/.env.example` zeigt. Swagger liegt unter `/swagger`. In Development
ist der HTTPS-Redirect bewusst deaktiviert, weil er vor `UseCors` greift und
damit CORS-Preflight und SignalR-Negotiate des Frontends brechen würde; wer
TLS lokal testen will, nimmt `dotnet run --launch-profile https`
(`https://localhost:5001`) und setzt `VITE_API_BASE_URL`/`VITE_RACE_HUB_URL`
entsprechend um.

Ein Development-only-Endpoint `POST /api/dev/simulate-kill` broadcastet ein
Beispiel-`TickerEvent` über den `RaceHub` — nützlich, um die Live-Update-Kette
(Dashboard-Ticker, Kill-Toasts, Gilden-Profil-Invalidierung) ohne echte
Warcraft-Logs-Zugangsdaten manuell zu testen.

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

**Frontend-Live-Anbindung** (nicht nur im Dashboard): Eine geteilte
SignalR-Verbindung (`lib/raceHubConnection.ts`) treibt drei unabhängige
Verbraucher — den Ticker im Dashboard, app-weite Kill-Toasts
(`hooks/useKillToasts.ts` + `components/KillToastStack.tsx`, gemountet in
`Layout.tsx`) und die Live-Invalidierung des gerade geöffneten Gilden-Profils
(`useGuildProfile` in `hooks/useGuild.ts` — reagiert nur auf `TickerEvent`s
der eigenen Gilde). `hooks/useHubConnectionStatus.ts` zeigt den
Verbindungsstatus als Badge im Header (Live/Verbindung wird aufgebaut/
Getrennt). Im Mock-Modus (`VITE_USE_MOCKS=true`) bleiben Toasts und
Live-Invalidierung bewusst inaktiv — ohne echten Hub gäbe es sonst
vorgetäuschte Live-Ereignisse; der Badge zeigt dort "Demo-Daten".

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

## PvP-Rating-Leiter (Beta)

Erster Schritt Richtung PvP-Esports (analog zur RWF-Idee: "Race to Rank 1 /
Gladiator" statt "Race to World First"). Bewusst als Beta markiert — überall
sichtbar per `<BetaBadge>` (Nav-Link, Seitentitel) — weil es noch **keine**
Blizzard-Battle.net-API-Anbindung gibt. `Data/PvpDemoSeeder.cs` füllt
`GET /api/pvp/ladder?bracket=<2v2|3v3|rbg|solo-shuffle>` mit rein fiktiven
Team-/Spielernamen statt erfundene Ratings für echte Personen vorzutäuschen —
gleiches Prinzip wie bei `DbSeeder`s fiktiver Live-Demo-Season.

Tier-Einstufung (Combatant → Challenger → Rival → Duelist → Elite →
Gladiator) läuft über feste Rating-Schwellen in `PvpController.TierFor` — eine
grobe Näherung, keine belastbare Einstufung, da die echten Cutoffs erst am
Season-Ende pro Bracket/Region von Blizzard festgelegt werden.
`components/RatingRail.tsx` spiegelt bewusst die Boss-Rail-Optik (Knoten,
Gold = erreicht, Ember-Glow = aktueller Tier) als visuelle Klammer zwischen
PvE- und PvP-Bereich.

Nächste Schritte für einen Vollausbau: eigenes `Player`-Entity (aktuell nur
Strings je Team), echte Ladder-Daten über die Battle.net-API, sowie
AWC-Turnier-Tracking (Bracket-Ansicht, Live-Match-Ticker) als eigener Ausbau
— siehe Konzeptideen in der Projekt-Historie.

## Design-System

Siehe `frontend/src/index.css` (`@theme`-Block) für die Farb- und
Typografie-Tokens des "Vanguard"-Designs (Obsidian/Türkis/Gold/Ember,
Cinzel/Barlow Condensed/Inter/JetBrains Mono).

## Version & Splash Screen

Aktuelle Version: **0.1.0** — einzige Quelle ist `frontend/package.json`
(`src/lib/version.ts` importiert sie von dort) und `<Version>` in
`VanguardTracker.Api.csproj`/`AppInfo.cs` fürs Backend; beim Hochzählen beide
Stellen synchron halten. Sichtbar im Frontend-Footer auf jeder Seite und auf
dem Splash Screen; im Backend über `GET /api/version` sowie im Swagger-Titel.

`components/SplashScreen.tsx` zeigt beim App-Start kurz (~1,8s) das
VANGUARD-Wortmark mit einlaufender Tagline und einem sechs-Punkte-Loader im
Boss-Rail-Motiv (dieselben Knoten wie in `BossRail.tsx`, hier nacheinander in
Gold aufleuchtend statt kill-Status) — bewusst als kleine visuelle
Selbstreferenz auf das Signature-Element des Designs. In `App.tsx` per
`showSplash`-State eingebunden, blendet über `opacity`+`pointer-events-none`
aus und wird danach aus dem Baum entfernt.

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

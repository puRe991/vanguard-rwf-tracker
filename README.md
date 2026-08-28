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

## Design-System

Siehe `frontend/src/index.css` (`@theme`-Block) für die Farb- und
Typografie-Tokens des "Vanguard"-Designs (Obsidian/Türkis/Gold/Ember,
Cinzel/Barlow Condensed/Inter/JetBrains Mono).

## Phasenplan

1. **MVP** (erledigt): EF-Core-Schema + manuelles Seeding, React-Dashboard
   gegen Mock- bzw. geseedete Daten, kein Live-Polling.
2. **Live-Tracking** (aktuell): SignalR-Hub + Warcraft-Logs-Polling für
   automatische Kill-/Pull-Erkennung der aktuellen Mythic-Race, siehe
   „Warcraft-Logs-Live-Tracking" unten.
3. Historie-Import für vergangene Seasons (ab Cataclysm via API, davor
   manuell/kuratiert).
4. Community-Beitrags-Workflow + Moderation (JWT-Auth ist im Backend
   bereits verdrahtet, `POST /api/kills/submit` erfordert `[Authorize]`).
5. Benachrichtigungen/Discord-Integration.

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

## Design-System

Siehe `frontend/src/index.css` (`@theme`-Block) für die Farb- und
Typografie-Tokens des "Vanguard"-Designs (Obsidian/Türkis/Gold/Ember,
Cinzel/Barlow Condensed/Inter/JetBrains Mono).

## Phasenplan

1. **MVP** (aktuell): EF-Core-Schema + manuelles Seeding, React-Dashboard
   gegen Mock- bzw. geseedete Daten, kein Live-Polling.
2. SignalR + Warcraft-Logs-Integration für automatisches Live-Tracking
   (`Services/WarcraftLogsPollingService.cs` ist als Gerüst angelegt).
3. Historie-Import für vergangene Seasons (ab Cataclysm via API, davor
   manuell/kuratiert).
4. Community-Beitrags-Workflow + Moderation (JWT-Auth ist im Backend
   bereits verdrahtet, `POST /api/kills/submit` erfordert `[Authorize]`).
5. Benachrichtigungen/Discord-Integration.

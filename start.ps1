#Requires -Version 5.1
<#
.SYNOPSIS
    Startet Vanguard (Backend + Frontend) inklusive Abhaengigkeits-Check.

.DESCRIPTION
    Prueft die Werkzeuge, stellt die Backend-Pakete wieder her, laesst
    frontend/scripts/setup.mjs die Node-Abhaengigkeiten installieren und
    verifizieren und startet danach beide Prozesse.

    Die Frontend-Installation wird nicht mehr bei jedem Start blind neu
    aufgesetzt: setup.mjs repariert hoechstens dreimal mit steigender Haerte
    und bricht danach mit einer konkreten Fehlermeldung ab, statt in eine
    Endlosschleife zu laufen.

.EXAMPLE
    .\start.ps1
    .\start.ps1 -SkipBackend
    .\start.ps1 -InstallOnly
#>
[CmdletBinding()]
param(
    [switch]$SkipBackend,
    [switch]$SkipFrontend,
    [switch]$InstallOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Die Konsole auf UTF-8 stellen, sonst werden Sonderzeichen dieses Skripts als
# Mojibake ausgegeben (Windows-Konsolen laufen per Default auf CP850/CP437).
try {
    [Console]::OutputEncoding = [System.Text.UTF8Encoding]::new()
} catch {
    # Aeltere Hosts koennen die Ausgabecodierung nicht umstellen - unkritisch,
    # das Skript verwendet ohnehin nur ASCII.
}

$root = $PSScriptRoot
$backendProject = Join-Path $root 'backend\VanguardTracker.Api'
$frontendDir = Join-Path $root 'frontend'

function Write-Ok      { param($m) Write-Host "[OK]      $m" -ForegroundColor Green }
function Write-Warn    { param($m) Write-Host "[WARNUNG] $m" -ForegroundColor Yellow }
function Write-Err     { param($m) Write-Host "[FEHLER]  $m" -ForegroundColor Red }
function Write-Section { param($m) Write-Host ""; Write-Host $m -ForegroundColor Cyan }

Write-Host "==============================================="
Write-Host "  Vanguard - Race to World First Tracker"
Write-Host "  Abhaengigkeits-Check und Start"
Write-Host "==============================================="
Write-Host ""

# --- Werkzeuge pruefen -----------------------------------------------------

$missing = @()

if (-not $SkipBackend) {
    if (Get-Command dotnet -ErrorAction SilentlyContinue) {
        Write-Ok ".NET SDK gefunden ($(& dotnet --version))"
    } else {
        Write-Err ".NET 8 SDK nicht gefunden - https://dotnet.microsoft.com/download/dotnet/8.0"
        $missing += 'dotnet'
    }
}

if (-not $SkipFrontend) {
    if (Get-Command node -ErrorAction SilentlyContinue) {
        $nodeVersion = (& node --version).TrimStart('v')
        $nodeMajor = [int](($nodeVersion -split '\.')[0])
        if ($nodeMajor -lt 20) {
            Write-Err "Node.js $nodeVersion ist zu alt - benoetigt wird Node 20 oder neuer."
            $missing += 'node'
        } else {
            Write-Ok "Node.js gefunden (v$nodeVersion)"
        }
    } else {
        Write-Err "Node.js nicht gefunden - https://nodejs.org/"
        $missing += 'node'
    }

    if (Get-Command npm -ErrorAction SilentlyContinue) {
        Write-Ok "npm gefunden ($(& npm --version))"
    } else {
        Write-Err "npm nicht gefunden (wird normalerweise mit Node.js installiert)."
        $missing += 'npm'
    }
}

if (-not $SkipBackend -and -not (Get-Command psql -ErrorAction SilentlyContinue)) {
    Write-Warn "psql nicht gefunden - die Datenbankverbindung wird nicht automatisch geprueft."
    Write-Host  "          Es muss eine erreichbare PostgreSQL-Instanz laufen mit:"
    Write-Host  "            Host=localhost  Port=5432  Datenbank=vanguard  Benutzer=vanguard"
    Write-Host  "          (siehe backend\VanguardTracker.Api\appsettings.json bzw. den lokalen"
    Write-Host  "           Override appsettings.Development.local.json)"
}

if ($missing.Count -gt 0) {
    Write-Host ""
    Write-Err "Fehlende Abhaengigkeiten: $($missing -join ', '). Abbruch."
    exit 1
}

Write-Host ""
Write-Host "Alle Werkzeuge vorhanden."

# --- Backend-Pakete --------------------------------------------------------

if (-not $SkipBackend) {
    Write-Section "Stelle Backend-Pakete wieder her (dotnet restore)..."
    & dotnet restore (Join-Path $root 'backend\VanguardTracker.sln')
    if ($LASTEXITCODE -ne 0) {
        Write-Err "dotnet restore fehlgeschlagen. Abbruch."
        exit 1
    }
    Write-Ok "Backend-Pakete bereit."
}

# --- Frontend-Pakete -------------------------------------------------------

if (-not $SkipFrontend) {
    Write-Section "Pruefe Frontend-Installation..."
    & node (Join-Path $frontendDir 'scripts\setup.mjs')
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Err "Frontend-Abhaengigkeiten konnten nicht hergestellt werden. Abbruch."
        exit 1
    }
    Write-Ok "Frontend-Pakete bereit."

    $envFile = Join-Path $frontendDir '.env'
    $envExample = Join-Path $frontendDir '.env.example'
    if (-not (Test-Path -LiteralPath $envFile) -and (Test-Path -LiteralPath $envExample)) {
        Copy-Item -LiteralPath $envExample -Destination $envFile
        Write-Ok "frontend\.env aus .env.example angelegt (VITE_USE_MOCKS=true)."
    }
}

if ($InstallOnly) {
    Write-Host ""
    Write-Ok "Installation abgeschlossen (-InstallOnly gesetzt, nichts gestartet)."
    exit 0
}

# --- Starten ---------------------------------------------------------------

$started = @()

if ($SkipBackend -and $SkipFrontend) {
    Write-Warn "-SkipBackend und -SkipFrontend zugleich gesetzt - es wird nichts gestartet."
    exit 0
}

if (-not $SkipBackend) {
    Write-Section "Starte Backend (http://localhost:5000)..."
    Start-Process -FilePath 'powershell' -ArgumentList @(
        '-NoExit', '-NoProfile', '-Command',
        "Set-Location -LiteralPath '$backendProject'; dotnet run --launch-profile http"
    ) | Out-Null
    $started += 'Backend  http://localhost:5000  (Swagger: /swagger)'
}

if (-not $SkipFrontend) {
    Write-Section "Starte Frontend (http://localhost:5173)..."
    Start-Process -FilePath 'powershell' -ArgumentList @(
        '-NoExit', '-NoProfile', '-Command',
        "Set-Location -LiteralPath '$frontendDir'; npm run dev"
    ) | Out-Null
    $started += 'Frontend http://localhost:5173'
}

Write-Host ""
Write-Host "==============================================="
foreach ($line in $started) { Write-Host "  $line" }
Write-Host "==============================================="
Write-Host "Laeuft jeweils in einem eigenen Fenster. Zum Beenden dort Strg+C."

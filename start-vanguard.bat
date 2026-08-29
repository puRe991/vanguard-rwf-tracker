@echo off
setlocal EnableDelayedExpansion

REM =====================================================================
REM  Vanguard — Windows-Start-Skript
REM  Prueft benoetigte Abhaengigkeiten (.NET SDK, Node.js, PostgreSQL),
REM  installiert fehlende npm-Pakete und startet Backend + Frontend
REM  jeweils in einem eigenen Fenster.
REM =====================================================================

set "ROOT=%~dp0"
set "BACKEND_DIR=%ROOT%backend\VanguardTracker.Api"
set "FRONTEND_DIR=%ROOT%frontend"

echo.
echo  ===============================================
echo    Vanguard — Race to World First Tracker
echo    Abhaengigkeits-Check und Start
echo  ===============================================
echo.

set "MISSING=0"

REM --- .NET SDK -----------------------------------------------------
where dotnet >nul 2>nul
if errorlevel 1 (
    echo [FEHLT]  .NET SDK wurde nicht gefunden.
    echo          Download: https://dotnet.microsoft.com/download/dotnet/8.0
    set "MISSING=1"
) else (
    for /f "delims=" %%v in ('dotnet --version 2^>nul') do set "DOTNET_VERSION=%%v"
    echo [OK]     .NET SDK gefunden ^(!DOTNET_VERSION!^)
    echo !DOTNET_VERSION! | findstr /r "^8\." >nul
    if errorlevel 1 (
        echo          Hinweis: Version 8.x wird erwartet, gefunden ist !DOTNET_VERSION! — ggf. Kompatibilitaetsprobleme.
    )
)

REM --- Node.js / npm --------------------------------------------------
where node >nul 2>nul
if errorlevel 1 (
    echo [FEHLT]  Node.js wurde nicht gefunden.
    echo          Download: https://nodejs.org/ ^(Version 20 oder neuer^)
    set "MISSING=1"
) else (
    for /f "delims=" %%v in ('node --version 2^>nul') do set "NODE_VERSION=%%v"
    echo [OK]     Node.js gefunden ^(!NODE_VERSION!^)
)

where npm >nul 2>nul
if errorlevel 1 (
    echo [FEHLT]  npm wurde nicht gefunden ^(kommt normalerweise mit Node.js^).
    set "MISSING=1"
) else (
    echo [OK]     npm gefunden
)

REM --- PostgreSQL -------------------------------------------------------
REM Verbindungsdaten aus backend\VanguardTracker.Api\appsettings.json:
REM Host=localhost;Port=5432;Database=vanguard;Username=vanguard;Password=changeme
set "PGHOST=localhost"
set "PGPORT=5432"
set "PGDATABASE=vanguard"
set "PGUSER=vanguard"
set "PGPASSWORD=changeme"

where psql >nul 2>nul
if errorlevel 1 (
    echo [WARNUNG] psql-Kommandozeilentool nicht gefunden — Datenbankverbindung wird nicht automatisch geprueft.
    echo           Stelle sicher, dass eine erreichbare PostgreSQL-Instanz laeuft mit:
    echo             Host=%PGHOST%  Port=%PGPORT%  Datenbank=%PGDATABASE%  Benutzer=%PGUSER%
    echo           ^(siehe backend\VanguardTracker.Api\appsettings.json^)
) else (
    psql -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %PGDATABASE% -c "SELECT 1;" >nul 2>nul
    if errorlevel 1 (
        echo [WARNUNG] Verbindung zu PostgreSQL ^(%PGHOST%:%PGPORT%, DB "%PGDATABASE%", User "%PGUSER%"^) fehlgeschlagen.
        echo           Versuche, Datenbank/Benutzer anzulegen ^(erfordert lokalen "postgres"-Superuser-Zugriff^)...
        psql -h %PGHOST% -p %PGPORT% -U postgres -c "DO $$ BEGIN IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'vanguard') THEN CREATE ROLE vanguard LOGIN PASSWORD 'changeme'; END IF; END $$;" 2>nul
        psql -h %PGHOST% -p %PGPORT% -U postgres -tc "SELECT 1 FROM pg_database WHERE datname = 'vanguard'" 2>nul | findstr "1" >nul
        if errorlevel 1 (
            psql -h %PGHOST% -p %PGPORT% -U postgres -c "CREATE DATABASE vanguard OWNER vanguard;" 2>nul
        )
        psql -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %PGDATABASE% -c "SELECT 1;" >nul 2>nul
        if errorlevel 1 (
            echo [FEHLT]   PostgreSQL ist nicht erreichbar oder die Zugangsdaten stimmen nicht.
            echo           Download: https://www.postgresql.org/download/windows/
            set "MISSING=1"
        ) else (
            echo [OK]      Datenbank "vanguard" wurde eingerichtet.
        )
    ) else (
        echo [OK]     PostgreSQL erreichbar ^(%PGHOST%:%PGPORT%, DB "%PGDATABASE%"^)
    )
)

echo.
if "%MISSING%"=="1" (
    echo Es fehlen Abhaengigkeiten ^(siehe oben^). Bitte installieren und dieses Skript erneut starten.
    pause
    exit /b 1
)

echo Alle Abhaengigkeiten vorhanden.
echo.

REM --- Backend-Pakete wiederherstellen ---------------------------------
echo Stelle Backend-Pakete wieder her ^(dotnet restore^)...
pushd "%BACKEND_DIR%"
dotnet restore
if errorlevel 1 (
    echo [FEHLER] "dotnet restore" ist fehlgeschlagen.
    popd
    pause
    exit /b 1
)
popd

REM --- Frontend-Pakete installieren -------------------------------------
if not exist "%FRONTEND_DIR%\node_modules" (
    echo Installiere Frontend-Pakete ^(npm install^)...
    pushd "%FRONTEND_DIR%"
    call npm install
    if errorlevel 1 (
        echo [FEHLER] "npm install" ist fehlgeschlagen.
        popd
        pause
        exit /b 1
    )
    popd
) else (
    echo [OK]     Frontend-Pakete bereits installiert ^(node_modules vorhanden^).
)

REM --- Frontend-Installation auf Funktionsfaehigkeit pruefen --------------
REM Bekannter npm-Bug bei optionalen Abhaengigkeiten (npm/cli#4828) installiert
REM manchmal die falsche native Rolldown/Vite-Bindung fuer die Plattform.
REM Symptom: "vite" bzw. "npm run dev" bricht sofort mit
REM "Cannot find native binding" ab. Wird hier automatisch erkannt und
REM durch eine saubere Neuinstallation repariert.
echo Pruefe Frontend-Installation...
pushd "%FRONTEND_DIR%"
call npx --no-install vite --version >"%TEMP%\vanguard_vite_check.txt" 2>&1
if errorlevel 1 (
    echo [WARNUNG] Frontend-Installation ist fehlerhaft ^(vermutlich npm-Bug mit optionalen Abhaengigkeiten^).
    echo           Entferne node_modules und package-lock.json und installiere neu...
    popd
    if exist "%FRONTEND_DIR%\node_modules" rmdir /s /q "%FRONTEND_DIR%\node_modules"
    if exist "%FRONTEND_DIR%\package-lock.json" del /f /q "%FRONTEND_DIR%\package-lock.json"
    pushd "%FRONTEND_DIR%"
    call npm install
    if errorlevel 1 (
        echo [FEHLER] Neuinstallation der Frontend-Pakete ist fehlgeschlagen.
        popd
        pause
        exit /b 1
    )
    call npx --no-install vite --version >"%TEMP%\vanguard_vite_check.txt" 2>&1
    if errorlevel 1 (
        echo [FEHLER] Frontend-Installation weiterhin fehlerhaft. Ausgabe:
        type "%TEMP%\vanguard_vite_check.txt"
        popd
        pause
        exit /b 1
    )
    echo [OK]     Neuinstallation erfolgreich.
) else (
    echo [OK]     Frontend-Installation funktionsfaehig.
)
popd

REM --- Frontend-.env anlegen, falls nicht vorhanden ----------------------
if not exist "%FRONTEND_DIR%\.env" (
    echo Lege frontend\.env an ^(zeigt auf lokales Backend, keine Mock-Daten^)...
    (
        echo VITE_API_BASE_URL=http://localhost:5000/api
        echo VITE_RACE_HUB_URL=http://localhost:5000/hubs/race
        echo VITE_USE_MOCKS=false
    ) > "%FRONTEND_DIR%\.env"
) else (
    echo [OK]     frontend\.env bereits vorhanden — wird nicht ueberschrieben.
)

echo.
echo Starte Backend und Frontend in eigenen Fenstern...
echo.

REM --- Backend starten ----------------------------------------------------
start "Vanguard API (Backend)" cmd /k "cd /d "%BACKEND_DIR%" && set ASPNETCORE_ENVIRONMENT=Development && dotnet run"

REM --- Warten, bis das Backend antwortet -----------------------------------
echo Warte auf Backend-Start ^(http://localhost:5000^)...
set "READY=0"
for /l %%i in (1,1,30) do (
    if "!READY!"=="0" (
        curl -s -o nul -w "%%{http_code}" http://localhost:5000/api/version > "%TEMP%\vanguard_status.txt" 2>nul
        set /p STATUS=<"%TEMP%\vanguard_status.txt"
        if "!STATUS!"=="200" (
            set "READY=1"
        ) else (
            timeout /t 2 /nobreak >nul
        )
    )
)
if "%READY%"=="0" (
    echo [WARNUNG] Backend antwortet nach 60 Sekunden noch nicht — pruefe das Backend-Fenster auf Fehler.
) else (
    echo [OK]     Backend laeuft.
)

REM --- Frontend starten -----------------------------------------------------
start "Vanguard Frontend" cmd /k "cd /d "%FRONTEND_DIR%" && npm run dev"

timeout /t 3 /nobreak >nul
start "" "http://localhost:5173"

echo.
echo  ===============================================
echo    Vanguard laeuft:
echo      Frontend:  http://localhost:5173
echo      Backend:   http://localhost:5000  ^(Swagger unter /swagger^)
echo    Zum Beenden einfach die beiden geoeffneten
echo    Fenster schliessen.
echo  ===============================================
echo.
pause

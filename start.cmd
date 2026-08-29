@echo off
REM Doppelklick-Starter fuer Windows. Ruft start.ps1 mit gelockerter
REM ExecutionPolicy auf, damit das Skript auch ohne signierte Policy laeuft.
setlocal
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0start.ps1" %*
set "code=%ERRORLEVEL%"
if not "%code%"=="0" (
  echo.
  echo Start fehlgeschlagen ^(Exit-Code %code%^). Fenster bleibt zum Nachlesen offen.
  pause
)
exit /b %code%

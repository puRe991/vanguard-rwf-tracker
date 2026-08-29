#!/usr/bin/env bash
# Startet Vanguard (Backend + Frontend) unter Linux/macOS.
# Windows-Pendant: start.ps1 bzw. start.cmd
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
backend_project="$root/backend/VanguardTracker.Api"
frontend_dir="$root/frontend"

skip_backend=0
skip_frontend=0
install_only=0
for arg in "$@"; do
  case "$arg" in
    --skip-backend)  skip_backend=1 ;;
    --skip-frontend) skip_frontend=1 ;;
    --install-only)  install_only=1 ;;
    -h|--help)
      echo "Usage: ./start.sh [--skip-backend] [--skip-frontend] [--install-only]"
      exit 0 ;;
    *)
      echo "Unbekannte Option: $arg" >&2
      exit 2 ;;
  esac
done

ok()   { printf '[OK]      %s\n' "$1"; }
warn() { printf '[WARNUNG] %s\n' "$1"; }
err()  { printf '[FEHLER]  %s\n' "$1" >&2; }

echo "==============================================="
echo "  Vanguard - Race to World First Tracker"
echo "  Abhaengigkeits-Check und Start"
echo "==============================================="
echo

missing=()

if [ "$skip_backend" -eq 0 ]; then
  if command -v dotnet >/dev/null 2>&1; then
    ok ".NET SDK gefunden ($(dotnet --version))"
  else
    err ".NET 8 SDK nicht gefunden - https://dotnet.microsoft.com/download/dotnet/8.0"
    missing+=(dotnet)
  fi
fi

if [ "$skip_frontend" -eq 1 ]; then
  :
elif command -v node >/dev/null 2>&1; then
  node_version="$(node --version)"
  node_major="${node_version#v}"
  node_major="${node_major%%.*}"
  if [ "$node_major" -lt 20 ]; then
    err "Node.js $node_version ist zu alt - benoetigt wird Node 20 oder neuer."
    missing+=(node)
  else
    ok "Node.js gefunden ($node_version)"
  fi
else
  err "Node.js nicht gefunden - https://nodejs.org/"
  missing+=(node)
fi

if [ "$skip_frontend" -eq 0 ]; then
  if command -v npm >/dev/null 2>&1; then
    ok "npm gefunden ($(npm --version))"
  else
    err "npm nicht gefunden (wird normalerweise mit Node.js installiert)."
    missing+=(npm)
  fi
fi

if [ "$skip_backend" -eq 0 ] && ! command -v psql >/dev/null 2>&1; then
  warn "psql nicht gefunden - die Datenbankverbindung wird nicht automatisch geprueft."
  echo  "          Es muss eine erreichbare PostgreSQL-Instanz laufen mit:"
  echo  "            Host=localhost  Port=5432  Datenbank=vanguard  Benutzer=vanguard"
  echo  "          (siehe backend/VanguardTracker.Api/appsettings.json)"
fi

if [ "${#missing[@]}" -gt 0 ]; then
  echo
  err "Fehlende Abhaengigkeiten: ${missing[*]}. Abbruch."
  exit 1
fi

echo
echo "Alle Werkzeuge vorhanden."

if [ "$skip_backend" -eq 0 ]; then
  echo
  echo "Stelle Backend-Pakete wieder her (dotnet restore)..."
  dotnet restore "$root/backend/VanguardTracker.sln"
  ok "Backend-Pakete bereit."
fi

if [ "$skip_frontend" -eq 0 ]; then
  echo
  echo "Pruefe Frontend-Installation..."
  node "$frontend_dir/scripts/setup.mjs"
  ok "Frontend-Pakete bereit."

  if [ ! -f "$frontend_dir/.env" ] && [ -f "$frontend_dir/.env.example" ]; then
    cp "$frontend_dir/.env.example" "$frontend_dir/.env"
    ok "frontend/.env aus .env.example angelegt (VITE_USE_MOCKS=true)."
  fi
fi

if [ "$install_only" -eq 1 ]; then
  echo
  ok "Installation abgeschlossen (--install-only gesetzt, nichts gestartet)."
  exit 0
fi

pids=()
cleanup() {
  for pid in "${pids[@]:-}"; do
    kill "$pid" 2>/dev/null || true
  done
}
trap cleanup EXIT INT TERM

if [ "$skip_backend" -eq 0 ]; then
  echo
  echo "Starte Backend (http://localhost:5000)..."
  ( cd "$backend_project" && dotnet run --launch-profile http ) &
  pids+=($!)
fi

if [ "$skip_frontend" -eq 0 ]; then
  echo
  echo "Starte Frontend (http://localhost:5173)..."
  ( cd "$frontend_dir" && npm run dev ) &
  pids+=($!)
fi

echo
echo "==============================================="
if [ "$skip_backend" -eq 0 ]; then  echo "  Backend  http://localhost:5000  (Swagger: /swagger)"; fi
if [ "$skip_frontend" -eq 0 ]; then echo "  Frontend http://localhost:5173"; fi
echo "==============================================="
echo "Zum Beenden Strg+C."

wait

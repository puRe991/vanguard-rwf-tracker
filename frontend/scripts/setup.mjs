#!/usr/bin/env node
// Installiert und verifiziert die Frontend-Abhaengigkeiten.
//
// Hintergrund: vite/rolldown, tailwindcss/oxide und lightningcss liefern ihre
// nativen Binaries als plattformspezifische optionalDependencies aus. npm hat
// einen bekannten Bug (https://github.com/npm/cli/issues/4828), bei dem diese
// optionalen Pakete uebersprungen werden, wenn ueber einen bereits vorhandenen
// node_modules-Baum installiert wird -- typischerweise, wenn package-lock.json
// auf einer anderen Plattform erzeugt wurde. Ergebnis: "Cannot find native
// binding", und zwar reproduzierbar bei jedem Start.
//
// Die naive Reparatur (node_modules + package-lock.json loeschen, npm install)
// ist unter Windows unzuverlaessig: laufende node/vite-Prozesse, Virenscanner
// und lange Pfade lassen das Loeschen fehlschlagen, npm installiert ueber die
// Reste hinweg -- und der Start laeuft in eine Endlosschleife.
//
// Dieses Skript ersetzt das durch eine begrenzte Reparaturleiter: pruefen,
// hoechstens drei Reparaturversuche mit zunehmender Haerte, danach eine
// konkrete Fehlermeldung. Es wiederholt sich nie endlos.
//
// Aufruf:
//   node scripts/setup.mjs            installieren + verifizieren + reparieren
//   node scripts/setup.mjs --verify   nur verifizieren (Exit 0/1)
//   node scripts/setup.mjs --check    nur pruefen, nicht reparieren (Exit 0/1)

import { spawnSync } from 'node:child_process'
import fs from 'node:fs'
import path from 'node:path'
import process from 'node:process'
import { fileURLToPath } from 'node:url'

const frontendDir = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..')
const nodeModules = path.join(frontendDir, 'node_modules')
const lockFile = path.join(frontendDir, 'package-lock.json')

// Module, die beim Dev-Start tatsaechlich geladen werden. Ein reiner
// Existenz-Check auf node_modules reicht nicht: die Ordner sind da, nur die
// nativen Bindings fehlen. Deshalb werden die Pakete wirklich importiert.
const REQUIRED_MODULES = [
  'vite',
  'rolldown',
  '@vitejs/plugin-react',
  '@tailwindcss/postcss',
  '@tailwindcss/oxide',
  'lightningcss',
  'react',
  'react-dom',
]

const isWindows = process.platform === 'win32'

// Pakete der Toolchain, die fuer 32-Bit (ia32) ueberhaupt kein natives Binding
// veroeffentlichen. Auf so einem System ist die Installation nicht "kaputt" --
// es gibt schlicht nichts zu installieren, und jede Reparatur waere sinnlos.
// Stand geprueft gegen die in package-lock.json gepinnten Versionen.
const NO_IA32_BUILD = [
  ['rolldown (Bundler von Vite 8)', 'kein ia32-Binding, kein WASM-Fallback'],
  ['lightningcss (via @tailwindcss/postcss)', 'kein ia32-Binding, kein WASM-Fallback'],
  ['@tailwindcss/oxide', 'kein ia32-Binding (nur ein langsamer WASM-Fallback)'],
]

/**
 * Prueft, ob die Toolchain auf dieser Architektur ueberhaupt laufen kann.
 * Ohne diesen Check meldet der Import-Test "Cannot find native binding" -- also
 * exakt dieselbe Meldung wie beim npm-Bug -- und die Reparaturleiter wuerde
 * dreimal vergeblich neu installieren.
 */
function checkArchitecture() {
  if (process.arch !== 'ia32') return true

  // Unter Windows setzt WOW64 diese Variable nur, wenn ein 32-Bit-Prozess auf
  // einem 64-Bit-Betriebssystem laeuft.
  const osIs64Bit = Boolean(process.env.PROCESSOR_ARCHITEW6432)

  log('')
  log('FEHLER: Node.js laeuft hier als 32-Bit-Prozess (process.arch = ia32).')
  log('')
  log('Fuer diese Architektur gibt es die noetigen nativen Binaries nicht:')
  for (const [name, detail] of NO_IA32_BUILD) log(`  - ${name}: ${detail}`)
  log('')
  log('Das laesst sich nicht durch Neuinstallieren beheben - die Pakete')
  log('existieren fuer ia32 schlicht nicht.')
  log('')

  if (osIs64Bit) {
    log(`Das Betriebssystem ist aber 64-Bit (${process.env.PROCESSOR_ARCHITEW6432}).`)
    log('Es ist nur ein 32-Bit-Node installiert. Loesung:')
    log('  1. Node.js 32-Bit deinstallieren (liegt meist unter')
    log('     C:\\Program Files (x86)\\nodejs).')
    log('  2. Node.js 20 LTS oder neuer als x64-Installer von https://nodejs.org/')
    log('     installieren ("Windows Installer (.msi) 64-bit").')
    log('  3. Neue Konsole oeffnen, mit "node -p process.arch" pruefen (erwartet: x64).')
    log('  4. Danach erneut: npm run setup')
  } else {
    log('Das Betriebssystem selbst ist offenbar 32-Bit. Dann hilft nur eines von:')
    log('  - Die Toolchain auf 32-Bit-faehige Versionen zurueckstufen:')
    log('    Vite 7 (nutzt Rollup + esbuild, beide mit ia32-Binding) statt Vite 8,')
    log('    und Tailwind CSS 3 (reines JavaScript) statt Tailwind 4.')
    log('  - Oder das Frontend auf einem 64-Bit-Rechner bauen (npm run build) und')
    log('    nur den fertigen dist/-Ordner ausliefern; das Backend (.NET 8) laeuft')
    log('    auch unter 32-Bit-Windows.')
  }
  log('')
  return false
}

function log(message) {
  process.stdout.write(`${message}\n`)
}

/** Importiert alle Pflichtmodule im Kontext des frontend/-Verzeichnisses. */
async function verify() {
  const failures = []
  for (const name of REQUIRED_MODULES) {
    try {
      // Bare Specifier: Node laeuft die Verzeichnisse hoch und landet in
      // frontend/node_modules -- unabhaengig vom aktuellen Arbeitsverzeichnis.
      await import(name)
    } catch (error) {
      failures.push({ name, message: String(error?.message ?? error).split('\n')[0] })
    }
  }
  return failures
}

/** Verifikation in einem Kindprozess, damit ein harter Ladefehler den Lauf nicht abbricht. */
function verifyInChildProcess() {
  const result = spawnSync(process.execPath, [fileURLToPath(import.meta.url), '--verify'], {
    cwd: frontendDir,
    stdio: ['ignore', 'pipe', 'pipe'],
    encoding: 'utf8',
  })
  return {
    ok: result.status === 0,
    output: `${result.stdout ?? ''}${result.stderr ?? ''}`.trim(),
  }
}

function runNpm(args) {
  log(`  > npm ${args.join(' ')}`)
  const result = spawnSync(isWindows ? 'npm.cmd' : 'npm', args, {
    cwd: frontendDir,
    stdio: 'inherit',
    // Ohne shell findet Windows npm.cmd je nach PATHEXT-Konfiguration nicht.
    shell: isWindows,
  })
  return result.status === 0
}

/**
 * Loescht node_modules so robust wie moeglich. Erst umbenennen (schlaegt sofort
 * fehl, wenn der Ordner in Benutzung ist, statt halb geloescht zurueckzubleiben),
 * dann den umbenannten Ordner entfernen.
 */
function removeNodeModules() {
  if (!fs.existsSync(nodeModules)) return true

  const trash = path.join(frontendDir, `.trash-node_modules-${Date.now()}`)
  try {
    fs.renameSync(nodeModules, trash)
  } catch (error) {
    log(`  ! node_modules konnte nicht umbenannt werden: ${error.code ?? error.message}`)
    try {
      fs.rmSync(nodeModules, { recursive: true, force: true, maxRetries: 5, retryDelay: 300 })
      return !fs.existsSync(nodeModules)
    } catch {
      return false
    }
  }

  try {
    fs.rmSync(trash, { recursive: true, force: true, maxRetries: 5, retryDelay: 300 })
  } catch {
    // Der Ordner ist bereits aus dem Weg; Reste stoeren npm nicht mehr.
    log(`  ! Rest-Ordner ${path.basename(trash)} konnte nicht geloescht werden - bitte spaeter manuell entfernen.`)
  }
  return true
}

function removeLockFile() {
  try {
    fs.rmSync(lockFile, { force: true })
    return true
  } catch {
    return false
  }
}

// Reparaturleiter, von schonend nach hart. Jede Stufe laeuft hoechstens einmal.
const REPAIRS = [
  {
    label: 'npm ci (sauberer Install aus package-lock.json)',
    run: () => runNpm(['ci']),
  },
  {
    label: 'node_modules entfernen, dann npm ci',
    run: () => removeNodeModules() && runNpm(['ci']),
  },
  {
    label: 'node_modules und package-lock.json entfernen, dann npm install',
    run: () => removeNodeModules() && removeLockFile() && runNpm(['install']),
  },
]

function reportFailure(output) {
  log('')
  log('FEHLER: Die Frontend-Installation ist nach allen Reparaturversuchen defekt.')
  if (output) {
    log('')
    log('Details:')
    for (const line of output.split('\n')) log(`  ${line}`)
  }
  log('')
  log('Naechste Schritte:')
  log('  1. Alle laufenden node-/vite-Prozesse beenden (Windows: taskkill /f /im node.exe).')
  log('  2. Ordner frontend\\node_modules im Virenscanner ausnehmen (Windows Defender sperrt')
  log('     beim Entpacken gelegentlich .node-Dateien).')
  log('  3. Projekt in einen kurzen Pfad legen (z. B. C:\\dev\\vanguard); Pfade > 260 Zeichen')
  log('     lassen das Entpacken der nativen Binaries fehlschlagen.')
  log('  4. Danach erneut: cd frontend && npm ci')
  log('')
  log(`Plattform: ${process.platform}/${process.arch}, Node ${process.version}`)
}

async function main() {
  const args = process.argv.slice(2)

  if (args.includes('--verify')) {
    const failures = await verify()
    for (const failure of failures) log(`${failure.name}: ${failure.message}`)
    process.exit(failures.length === 0 ? 0 : 1)
  }

  const checkOnly = args.includes('--check')

  // Zuerst die Architektur, sonst diagnostiziert die Reparaturleiter einen
  // npm-Bug, wo in Wahrheit gar kein passendes Paket existiert.
  if (!checkArchitecture()) process.exit(1)

  if (!fs.existsSync(nodeModules)) {
    if (checkOnly) {
      log('node_modules fehlt.')
      process.exit(1)
    }
    log('Frontend-Pakete werden installiert...')
    if (!runNpm(['ci'])) {
      log('  ! npm ci fehlgeschlagen, versuche npm install...')
      removeLockFile()
      runNpm(['install'])
    }
  }

  let { ok, output } = verifyInChildProcess()
  if (ok) {
    log('Frontend-Installation ist vollstaendig.')
    return
  }

  if (checkOnly) {
    log('Frontend-Installation ist unvollstaendig.')
    if (output) log(output)
    process.exit(1)
  }

  log('Frontend-Installation ist unvollstaendig:')
  for (const line of output.split('\n')) log(`  ${line}`)

  for (const [index, repair] of REPAIRS.entries()) {
    log('')
    log(`Reparaturversuch ${index + 1}/${REPAIRS.length}: ${repair.label}`)
    if (!repair.run()) {
      log('  ! Versuch fehlgeschlagen.')
      continue
    }
    ;({ ok, output } = verifyInChildProcess())
    if (ok) {
      log('Frontend-Installation repariert.')
      return
    }
    log('  ! Verifikation weiterhin negativ.')
  }

  reportFailure(output)
  process.exit(1)
}

main().catch((error) => {
  log(`Unerwarteter Fehler in setup.mjs: ${error?.stack ?? error}`)
  process.exit(1)
})

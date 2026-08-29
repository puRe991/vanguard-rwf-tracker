import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_RACE_HUB_URL ?? '/hubs/race';

export type HubConnectionStatus = 'connecting' | 'connected' | 'reconnecting' | 'disconnected';

// Backoff für den *initialen* Verbindungsaufbau: withAutomaticReconnect greift erst,
// nachdem einmal erfolgreich verbunden wurde. Ohne eigenen Retry bliebe die App bis
// zum Reload tot, wenn das Backend beim ersten Laden noch nicht erreichbar war.
const RETRY_DELAYS_MS = [2_000, 5_000, 10_000, 30_000];

let connection: signalR.HubConnection | null = null;
let starting = false;

let status: HubConnectionStatus = 'connecting';
const statusListeners = new Set<() => void>();

function setStatus(next: HubConnectionStatus) {
  if (next === status) return;
  status = next;
  for (const listener of statusListeners) listener();
}

function startWithRetry(conn: signalR.HubConnection, attempt = 0) {
  // start() wirft, wenn die Verbindung nicht im Zustand Disconnected ist — und
  // onclose kann parallel zum laufenden Retry feuern.
  if (starting || conn.state !== signalR.HubConnectionState.Disconnected) return;

  starting = true;
  setStatus('connecting');

  conn
    .start()
    .then(() => {
      starting = false;
      setStatus('connected');
    })
    .catch((err: unknown) => {
      starting = false;
      setStatus('disconnected');
      console.error('SignalR-Verbindung fehlgeschlagen', err);

      const delay = RETRY_DELAYS_MS[Math.min(attempt, RETRY_DELAYS_MS.length - 1)];
      setTimeout(() => startWithRetry(conn, attempt + 1), delay);
    });
}

/**
 * Eine geteilte SignalR-Verbindung zum RaceHub für die ganze App — vermeidet,
 * dass Ticker und Live-Race-Invalidierung je eine eigene Verbindung aufbauen.
 */
export function getRaceHubConnection(): signalR.HubConnection {
  if (!connection) {
    const conn = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    // Genau einmal registriert: die Verbindung ist app-weit geteilt und SignalR
    // bietet zu diesen Callbacks kein Gegenstück zum Abmelden. Pro Komponente zu
    // registrieren würde sie bei jedem Mount dauerhaft anhäufen.
    conn.onreconnecting(() => setStatus('reconnecting'));
    conn.onreconnected(() => setStatus('connected'));
    conn.onclose(() => {
      setStatus('disconnected');
      startWithRetry(conn, 0);
    });

    connection = conn;
    startWithRetry(conn);
  }
  return connection;
}

/** Aktueller Verbindungsstatus als Snapshot für useSyncExternalStore. */
export function getHubStatus(): HubConnectionStatus {
  return status;
}

export function subscribeHubStatus(listener: () => void): () => void {
  statusListeners.add(listener);
  return () => {
    statusListeners.delete(listener);
  };
}

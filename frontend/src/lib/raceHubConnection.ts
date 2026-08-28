import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_RACE_HUB_URL ?? '/hubs/race';

let connection: signalR.HubConnection | null = null;

/**
 * Eine geteilte SignalR-Verbindung zum RaceHub für die ganze App — vermeidet,
 * dass Ticker und Live-Race-Invalidierung je eine eigene Verbindung aufbauen.
 */
export function getRaceHubConnection(): signalR.HubConnection {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    connection.start().catch((err) => console.error('SignalR-Verbindung fehlgeschlagen', err));
  }
  return connection;
}

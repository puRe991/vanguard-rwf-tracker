import { useEffect, useSyncExternalStore } from 'react';
import {
  getHubStatus,
  getRaceHubConnection,
  subscribeHubStatus,
  type HubConnectionStatus as RawHubStatus,
} from '../lib/raceHubConnection';

const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';

export type HubConnectionStatus = RawHubStatus | 'mock';

/**
 * Live-Verbindungsstatus des RaceHub — im Mock-Modus konstant 'mock'.
 *
 * Der Status liegt im Modul-Store neben der geteilten Verbindung, nicht in
 * lokalem State: die on*-Callbacks von SignalR lassen sich nicht wieder
 * abmelden, dürfen also nicht pro Komponenten-Mount registriert werden.
 * useSyncExternalStore liest ihn ohne setState-im-Effect.
 */
export function useHubConnectionStatus(): HubConnectionStatus {
  const status = useSyncExternalStore(subscribeHubStatus, getHubStatus);

  useEffect(() => {
    if (USE_MOCKS) return;
    getRaceHubConnection();
  }, []);

  return USE_MOCKS ? 'mock' : status;
}

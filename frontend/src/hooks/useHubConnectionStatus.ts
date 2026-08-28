import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { getRaceHubConnection } from '../lib/raceHubConnection';

const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';

export type HubConnectionStatus = 'mock' | 'connecting' | 'connected' | 'reconnecting' | 'disconnected';

function fromState(state: signalR.HubConnectionState): HubConnectionStatus {
  switch (state) {
    case signalR.HubConnectionState.Connected:
      return 'connected';
    case signalR.HubConnectionState.Connecting:
    case signalR.HubConnectionState.Reconnecting:
      return 'reconnecting';
    default:
      return 'disconnected';
  }
}

/** Live-Verbindungsstatus des RaceHub — im Mock-Modus konstant 'mock'. */
export function useHubConnectionStatus(): HubConnectionStatus {
  const [status, setStatus] = useState<HubConnectionStatus>(USE_MOCKS ? 'mock' : 'connecting');

  useEffect(() => {
    if (USE_MOCKS) return;

    const connection = getRaceHubConnection();
    setStatus(fromState(connection.state));

    connection.onreconnecting(() => setStatus('reconnecting'));
    connection.onreconnected(() => setStatus('connected'));
    connection.onclose(() => setStatus('disconnected'));

    // Fallback-Polling: falls der initiale connection.start() erst nach dem Mount
    // durchläuft, greifen die on*-Callbacks oben nicht rückwirkend.
    const interval = setInterval(() => setStatus(fromState(connection.state)), 2000);
    return () => clearInterval(interval);
  }, []);

  return status;
}

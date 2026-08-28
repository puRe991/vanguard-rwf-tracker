import { useQuery } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { fetchCurrentRace, fetchLiveTicker } from '../lib/api';
import type { LiveTickerEvent } from '../types';

const HUB_URL = import.meta.env.VITE_RACE_HUB_URL ?? '/hubs/race';
const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';

export function useLiveRace() {
  return useQuery({
    queryKey: ['race', 'current'],
    queryFn: fetchCurrentRace,
    refetchInterval: USE_MOCKS ? false : 30_000,
  });
}

export function useLiveTicker() {
  const initial = useQuery({
    queryKey: ['race', 'ticker'],
    queryFn: fetchLiveTicker,
  });
  const [liveEvents, setLiveEvents] = useState<LiveTickerEvent[]>([]);

  useEffect(() => {
    if (USE_MOCKS) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    connection.on('TickerEvent', (event: LiveTickerEvent) => {
      setLiveEvents((prev) => [event, ...prev].slice(0, 50));
    });

    connection.start().catch((err) => console.error('SignalR-Verbindung fehlgeschlagen', err));

    return () => {
      connection.stop();
    };
  }, []);

  const events = [...liveEvents, ...(initial.data ?? [])];
  return { ...initial, events };
}

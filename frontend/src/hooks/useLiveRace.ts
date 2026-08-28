import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import { fetchCurrentRace, fetchLiveTicker } from '../lib/api';
import { getRaceHubConnection } from '../lib/raceHubConnection';
import type { LiveTickerEvent } from '../types';

const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';

export function useLiveRace() {
  const queryClient = useQueryClient();
  const query = useQuery({
    queryKey: ['race', 'current'],
    queryFn: fetchCurrentRace,
    // SignalR treibt Aktualisierungen live über RaceUpdated; das Intervall ist nur
    // ein Fallback, falls die Hub-Verbindung mal ausfällt.
    refetchInterval: USE_MOCKS ? false : 60_000,
  });

  useEffect(() => {
    if (USE_MOCKS) return;

    const connection = getRaceHubConnection();
    const onRaceUpdated = () => {
      queryClient.invalidateQueries({ queryKey: ['race', 'current'] });
    };

    connection.on('RaceUpdated', onRaceUpdated);
    return () => {
      connection.off('RaceUpdated', onRaceUpdated);
    };
  }, [queryClient]);

  return query;
}

export function useLiveTicker() {
  const initial = useQuery({
    queryKey: ['race', 'ticker'],
    queryFn: fetchLiveTicker,
  });
  const [liveEvents, setLiveEvents] = useState<LiveTickerEvent[]>([]);

  useEffect(() => {
    if (USE_MOCKS) return;

    const connection = getRaceHubConnection();
    const onTickerEvent = (event: LiveTickerEvent) => {
      setLiveEvents((prev) => [event, ...prev].slice(0, 50));
    };

    connection.on('TickerEvent', onTickerEvent);
    return () => {
      connection.off('TickerEvent', onTickerEvent);
    };
  }, []);

  const events = [...liveEvents, ...(initial.data ?? [])];
  return { ...initial, events };
}

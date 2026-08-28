import { useEffect } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { fetchGuild, fetchGuildProfile } from '../lib/api';
import { getRaceHubConnection } from '../lib/raceHubConnection';
import type { LiveTickerEvent } from '../types';

const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';

export function useGuild(id: string | undefined) {
  return useQuery({
    queryKey: ['guild', id],
    queryFn: () => fetchGuild(id!),
    enabled: !!id,
  });
}

export function useGuildProfile(id: string | undefined) {
  const queryClient = useQueryClient();
  const query = useQuery({
    queryKey: ['guild', id, 'profile'],
    queryFn: () => fetchGuildProfile(id!),
    enabled: !!id,
  });

  const guildName = query.data?.guild.name;

  // Wenn während des Betrachtens ein neuer Kill dieser Gilde reinkommt, Profil +
  // aktuellen Fortschritt neu laden statt auf den nächsten Seitenaufruf zu warten.
  useEffect(() => {
    if (USE_MOCKS || !id || !guildName) return;

    const connection = getRaceHubConnection();
    const onTickerEvent = (event: LiveTickerEvent) => {
      if (event.guildName !== guildName) return;
      queryClient.invalidateQueries({ queryKey: ['guild', id, 'profile'] });
      queryClient.invalidateQueries({ queryKey: ['guild', id] });
    };

    connection.on('TickerEvent', onTickerEvent);
    return () => {
      connection.off('TickerEvent', onTickerEvent);
    };
  }, [id, guildName, queryClient]);

  return query;
}

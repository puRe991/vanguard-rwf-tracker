import { useQuery } from '@tanstack/react-query';
import { fetchGuild, fetchGuildProfile } from '../lib/api';

export function useGuild(id: string | undefined) {
  return useQuery({
    queryKey: ['guild', id],
    queryFn: () => fetchGuild(id!),
    enabled: !!id,
  });
}

export function useGuildProfile(id: string | undefined) {
  return useQuery({
    queryKey: ['guild', id, 'profile'],
    queryFn: () => fetchGuildProfile(id!),
    enabled: !!id,
  });
}

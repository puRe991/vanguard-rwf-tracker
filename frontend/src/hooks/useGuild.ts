import { useQuery } from '@tanstack/react-query';
import { fetchGuild } from '../lib/api';

export function useGuild(id: string | undefined) {
  return useQuery({
    queryKey: ['guild', id],
    queryFn: () => fetchGuild(id!),
    enabled: !!id,
  });
}

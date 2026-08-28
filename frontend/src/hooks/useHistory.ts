import { useQuery } from '@tanstack/react-query';
import { fetchHistory } from '../lib/api';

export function useHistory(params?: { expansion?: string; season?: number }) {
  return useQuery({
    queryKey: ['history', params],
    queryFn: () => fetchHistory(params),
  });
}

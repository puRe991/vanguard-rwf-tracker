import { useQuery } from '@tanstack/react-query';
import { fetchPvpLadder } from '../lib/api';
import type { PvpBracket } from '../types';

export function usePvpLadder(bracket: PvpBracket) {
  return useQuery({
    queryKey: ['pvp', 'ladder', bracket],
    queryFn: () => fetchPvpLadder(bracket),
  });
}

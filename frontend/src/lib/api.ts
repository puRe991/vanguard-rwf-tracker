import type { BossPullSeries, GuildRaceEntry, HistoryTier, LiveTickerEvent } from '../types';
import { mockCurrentRace, mockHistory, mockPullSeries, mockTicker } from '../mocks/data';

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? '/api';

// MVP-Phase: Backend liefert noch keine Live-Daten, daher Mock-Fallback.
// Sobald die API steht, ersetzen die fetch-Aufrufe die Mock-Returns unten.
const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';

async function getJson<T>(path: string): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`);
  if (!res.ok) {
    throw new Error(`API-Fehler ${res.status} bei ${path}`);
  }
  return res.json() as Promise<T>;
}

export async function fetchCurrentRace(): Promise<GuildRaceEntry[]> {
  if (USE_MOCKS) return mockCurrentRace;
  return getJson<GuildRaceEntry[]>('/races/current');
}

export async function fetchLiveTicker(): Promise<LiveTickerEvent[]> {
  if (USE_MOCKS) return mockTicker;
  return getJson<LiveTickerEvent[]>('/races/current/ticker');
}

export async function fetchHistory(params?: {
  expansion?: string;
  season?: number;
}): Promise<HistoryTier[]> {
  if (USE_MOCKS) {
    return mockHistory.filter((t) => {
      if (params?.expansion && t.expansion !== params.expansion) return false;
      if (params?.season && t.season !== params.season) return false;
      return true;
    });
  }
  const query = new URLSearchParams();
  if (params?.expansion) query.set('expansion', params.expansion);
  if (params?.season) query.set('season', String(params.season));
  return getJson<HistoryTier[]>(`/history?${query.toString()}`);
}

export async function fetchBossPulls(bossId: string): Promise<BossPullSeries[]> {
  if (USE_MOCKS) return mockPullSeries;
  return getJson<BossPullSeries[]>(`/bosses/${bossId}/pulls`);
}

export async function fetchGuild(id: string): Promise<GuildRaceEntry | undefined> {
  if (USE_MOCKS) return mockCurrentRace.find((g) => g.guild.id === id);
  return getJson<GuildRaceEntry>(`/guilds/${id}`);
}

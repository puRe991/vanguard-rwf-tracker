export type KillStatus = 'confirmed' | 'unconfirmed';

export interface Boss {
  id: string;
  raidId: string;
  name: string;
  order: number;
}

export interface BossProgress extends Boss {
  status: 'killed' | 'active' | 'locked';
  pullCount?: number;
  killedAt?: string;
}

export interface Guild {
  id: string;
  name: string;
  region: string;
  foundedYear?: number;
}

export interface GuildRaceEntry {
  guild: Guild;
  rank: number;
  bosses: BossProgress[];
  bossesKilled: number;
  totalPulls: number;
  lastKillAt?: string;
}

export interface Raid {
  id: string;
  seasonId: string;
  name: string;
  bossCount: number;
  normalOpenAt?: string;
  heroicOpenAt?: string;
  mythicOpenAt?: string;
}

export interface Season {
  id: string;
  expansionId: string;
  number: number;
  startAt: string;
  endAt?: string;
}

export interface Expansion {
  id: string;
  name: string;
  releaseDate: string;
}

export interface Kill {
  id: string;
  bossId: string;
  guildId: string;
  timestamp: string;
  pullCount: number;
  sourceUrl?: string;
  status: KillStatus;
}

export interface HistoryBoss {
  name: string;
  order: number;
  killed: boolean;
}

export interface HistoryTier {
  expansion: string;
  season: number;
  raidName: string;
  worldFirstGuild: string;
  pullCount: number;
  killDate: string;
  bosses: HistoryBoss[];
}

export interface LiveTickerEvent {
  id: string;
  guildName: string;
  bossName: string;
  message: string;
  timestamp: string;
  kind: 'kill' | 'pull-milestone' | 'live-start';
}

export interface PullSeriesPoint {
  pullNumber: number;
  timestamp: string;
}

export interface BossPullSeries {
  guild: Guild;
  points: PullSeriesPoint[];
  killed: boolean;
}

export type GuildLifecycleStatus = 'active' | 'disbanded' | 'retired' | 'unknown';

export interface GuildLinks {
  twitch?: string;
  youTube?: string;
  twitter?: string;
  website?: string;
}

export interface GuildHistoryKill {
  expansion: string;
  raidName: string;
  bossName: string;
  killDate: string;
  pullCount: number;
  sourceUrl?: string;
}

export interface GuildProfile {
  guild: Guild;
  status: GuildLifecycleStatus;
  disbandedYear?: number;
  bio?: string;
  links: GuildLinks;
  history: GuildHistoryKill[];
}

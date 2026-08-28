import type {
  BossPullSeries,
  GuildRaceEntry,
  HistoryTier,
  LiveTickerEvent,
} from '../types';

const bossNames = [
  'Vexamus',
  'The Iron Choir',
  'Thane Drakksar',
  'Sable Weaver',
  'Grim Custodian',
  'Twin Sovereigns',
  'Ashen Court',
  'Voidbound Herald',
];

function makeBosses(killedCount: number, activePullCount?: number) {
  return bossNames.map((name, i) => {
    if (i < killedCount) {
      return {
        id: `boss-${i}`,
        raidId: 'raid-current',
        name,
        order: i,
        status: 'killed' as const,
        killedAt: new Date(Date.now() - (bossNames.length - i) * 3600_000).toISOString(),
      };
    }
    if (i === killedCount) {
      return {
        id: `boss-${i}`,
        raidId: 'raid-current',
        name,
        order: i,
        status: 'active' as const,
        pullCount: activePullCount ?? 0,
      };
    }
    return {
      id: `boss-${i}`,
      raidId: 'raid-current',
      name,
      order: i,
      status: 'locked' as const,
    };
  });
}

export const mockCurrentRace: GuildRaceEntry[] = [
  {
    guild: { id: 'g1', name: 'Liquid', region: 'EU' },
    rank: 1,
    bosses: makeBosses(6, 214),
    bossesKilled: 6,
    totalPulls: 892,
    lastKillAt: new Date(Date.now() - 3600_000).toISOString(),
  },
  {
    guild: { id: 'g2', name: 'Echo', region: 'EU' },
    rank: 2,
    bosses: makeBosses(6, 341),
    bossesKilled: 6,
    totalPulls: 951,
    lastKillAt: new Date(Date.now() - 5400_000).toISOString(),
  },
  {
    guild: { id: 'g3', name: 'Complexity Limit', region: 'NA' },
    rank: 3,
    bosses: makeBosses(5, 88),
    bossesKilled: 5,
    totalPulls: 704,
    lastKillAt: new Date(Date.now() - 9000_000).toISOString(),
  },
  {
    guild: { id: 'g4', name: 'BDGG', region: 'EU' },
    rank: 4,
    bosses: makeBosses(5, 302),
    bossesKilled: 5,
    totalPulls: 812,
    lastKillAt: new Date(Date.now() - 12_000_000).toISOString(),
  },
];

export const mockTicker: LiveTickerEvent[] = [
  {
    id: 't1',
    guildName: 'Liquid',
    bossName: 'Grim Custodian',
    message: 'Liquid startet den nächsten Pull-Block (Pull #215)',
    timestamp: new Date(Date.now() - 5 * 60_000).toISOString(),
    kind: 'pull-milestone',
  },
  {
    id: 't2',
    guildName: 'Echo',
    bossName: 'Sable Weaver',
    message: 'Echo besiegt Sable Weaver — Boss 6/8 down',
    timestamp: new Date(Date.now() - 90 * 60_000).toISOString(),
    kind: 'kill',
  },
  {
    id: 't3',
    guildName: 'Complexity Limit',
    bossName: 'Grim Custodian',
    message: 'Complexity Limit geht live',
    timestamp: new Date(Date.now() - 150 * 60_000).toISOString(),
    kind: 'live-start',
  },
];

export const mockHistory: HistoryTier[] = [
  {
    expansion: 'The War Within',
    season: 2,
    raidName: 'Nerub-ar Sanctum',
    worldFirstGuild: 'Liquid',
    pullCount: 892,
    killDate: '2025-03-14',
  },
  {
    expansion: 'Dragonflight',
    season: 4,
    raidName: 'Amirdrassil',
    worldFirstGuild: 'Echo',
    pullCount: 634,
    killDate: '2023-12-01',
  },
  {
    expansion: 'Shadowlands',
    season: 4,
    raidName: 'Sepulcher of the First Ones',
    worldFirstGuild: 'Echo',
    pullCount: 1063,
    killDate: '2022-04-01',
  },
  {
    expansion: 'Cataclysm',
    season: 1,
    raidName: 'Firelands',
    worldFirstGuild: 'Paragon',
    pullCount: 219,
    killDate: '2011-07-14',
  },
  {
    expansion: 'Wrath of the Lich King',
    season: 3,
    raidName: 'Icecrown Citadel',
    worldFirstGuild: 'Paragon',
    pullCount: 209,
    killDate: '2010-05-06',
  },
  // Vanilla: keine API-Abdeckung, community-kuratiert nach Method
  // ("Vanilla Raid History of World Firsts", method.gg/raid-history).
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Onyxia’s Lair',
    worldFirstGuild: 'Ruined',
    pullCount: 0,
    killDate: '2005-01-30',
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Molten Core',
    worldFirstGuild: 'Ascent',
    pullCount: 0,
    killDate: '2005-04-25',
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Blackwing Lair',
    worldFirstGuild: 'Drama',
    pullCount: 0,
    killDate: '2005-09-26',
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Zul’Gurub',
    worldFirstGuild: '—',
    pullCount: 0,
    killDate: '',
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Ruins of Ahn’Qiraj',
    worldFirstGuild: '—',
    pullCount: 0,
    killDate: '',
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Temple of Ahn’Qiraj',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2006-04-25',
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Naxxramas',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2006-09-07',
  },
];

export const mockPullSeries: BossPullSeries[] = [
  {
    guild: { id: 'g1', name: 'Liquid', region: 'EU' },
    killed: true,
    points: Array.from({ length: 214 }, (_, i) => ({
      pullNumber: i + 1,
      timestamp: new Date(Date.now() - (214 - i) * 900_000).toISOString(),
    })),
  },
  {
    guild: { id: 'g2', name: 'Echo', region: 'EU' },
    killed: false,
    points: Array.from({ length: 341 }, (_, i) => ({
      pullNumber: i + 1,
      timestamp: new Date(Date.now() - (341 - i) * 900_000).toISOString(),
    })),
  },
];

import type {
  BossPullSeries,
  GuildRaceEntry,
  HistoryBoss,
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

function bossesOf(names: string[], killedCount: number): HistoryBoss[] {
  return names.map((name, i) => ({ name, order: i, killed: i < killedCount }));
}

export const mockHistory: HistoryTier[] = [
  {
    expansion: 'The War Within',
    season: 2,
    raidName: 'Nerub-ar Sanctum',
    worldFirstGuild: 'Liquid',
    pullCount: 892,
    killDate: '2025-03-14',
    bosses: [],
  },
  {
    expansion: 'Dragonflight',
    season: 4,
    raidName: 'Amirdrassil',
    worldFirstGuild: 'Echo',
    pullCount: 634,
    killDate: '2023-12-01',
    bosses: [],
  },
  {
    expansion: 'Shadowlands',
    season: 4,
    raidName: 'Sepulcher of the First Ones',
    worldFirstGuild: 'Echo',
    pullCount: 1063,
    killDate: '2022-04-01',
    bosses: [],
  },
  {
    expansion: 'Cataclysm',
    season: 1,
    raidName: 'Firelands',
    worldFirstGuild: 'Paragon',
    pullCount: 219,
    killDate: '2011-07-14',
    bosses: [],
  },
  {
    expansion: 'Wrath of the Lich King',
    season: 3,
    raidName: 'Icecrown Citadel',
    worldFirstGuild: 'Paragon',
    pullCount: 209,
    killDate: '2010-05-06',
    bosses: [],
  },
  // Vanilla: keine API-Abdeckung. World-First-Ergebnisse community-kuratiert nach
  // Method ("Vanilla Raid History of World Firsts", method.gg/raid-history);
  // Boss-Rosters sind dokumentierter Spiel-Content.
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Onyxia’s Lair',
    worldFirstGuild: 'Ruined',
    pullCount: 0,
    killDate: '2005-01-30',
    bosses: bossesOf(['Onyxia'], 1),
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Molten Core',
    worldFirstGuild: 'Ascent',
    pullCount: 0,
    killDate: '2005-04-25',
    bosses: bossesOf(
      [
        'Lucifron', 'Magmadar', 'Gehennas', 'Garr', 'Baron Geddon',
        'Shazzrah', 'Sulfuron Harbinger', 'Golemagg the Incinerator',
        'Majordomo Executus', 'Ragnaros',
      ],
      10,
    ),
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Blackwing Lair',
    worldFirstGuild: 'Drama',
    pullCount: 0,
    killDate: '2005-09-26',
    bosses: bossesOf(
      [
        'Razorgore the Untamed', 'Vaelastrasz the Corrupt', 'Broodlord Lashlayer',
        'Firemaw', 'Ebonroc', 'Flamegor', 'Chromaggus', 'Nefarian',
      ],
      8,
    ),
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Zul’Gurub',
    worldFirstGuild: '—',
    pullCount: 0,
    killDate: '',
    bosses: bossesOf(
      [
        'High Priestess Jeklik', 'High Priest Venoxis', 'High Priestess Mar\'li',
        'Bloodlord Mandokir', 'Gahz\'rilla', 'Wushoolay', 'Renataki', 'Hazza\'rah',
        'High Priest Thekal', 'High Priestess Arlokk', 'Jin\'do the Hexxer',
        'Hakkar the Soulflayer',
      ],
      0,
    ),
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Ruins of Ahn’Qiraj',
    worldFirstGuild: '—',
    pullCount: 0,
    killDate: '',
    bosses: bossesOf(
      [
        'Kurinnaxx', 'General Rajaxx', 'Moam', 'Buru the Gorger',
        'Ayamiss the Hunter', 'Ossirian the Unscarred',
      ],
      0,
    ),
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Temple of Ahn’Qiraj',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2006-04-25',
    bosses: bossesOf(
      [
        'The Prophet Skeram', 'Lord Kri', 'Princess Yauj', 'Vem',
        'Battleguard Sartura', 'Fankriss the Unyielding', 'Viscidus',
        'Princess Huhuran', 'Twin Emperors Vek\'lor and Veknilash', 'Ouro', 'C\'Thun',
      ],
      11,
    ),
  },
  {
    expansion: 'Classic',
    season: 1,
    raidName: 'Naxxramas',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2006-09-07',
    bosses: bossesOf(
      [
        'Anub\'Rekhan', 'Grand Widow Faerlina', 'Maexxna',
        'Noth the Plaguebringer', 'Heigan the Unclean', 'Loatheb',
        'Instructor Razuvious', 'Gothik the Harvester', 'The Four Horsemen',
        'Patchwerk', 'Grobbulus', 'Gluth', 'Thaddius',
        'Sapphiron', 'Kel\'Thuzad',
      ],
      15,
    ),
  },
  // The Burning Crusade: World-First-Ergebnisse community-kuratiert nach Method
  // ("The Burning Crusade Raid History", method.gg/raid-history/the-burning-crusade).
  {
    expansion: 'The Burning Crusade',
    season: 1,
    raidName: 'Karazhan',
    worldFirstGuild: '—',
    pullCount: 0,
    killDate: '',
    bosses: bossesOf(
      [
        'Attumen the Huntsman', 'Moroes', 'Maiden of Virtue', 'The Opera Event',
        'The Curator', 'Terestian Illhoof', 'Shade of Aran', 'Netherspite',
        'Chess Event', 'Prince Malchezaar', 'Nightbane',
      ],
      0,
    ),
  },
  {
    expansion: 'The Burning Crusade',
    season: 1,
    raidName: 'Gruul\'s Lair',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2007-02-03',
    bosses: bossesOf(['High King Maulgar', 'Gruul the Dragonkiller'], 2),
  },
  {
    expansion: 'The Burning Crusade',
    season: 1,
    raidName: 'Magtheridon\'s Lair',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2007-02-24',
    bosses: bossesOf(['Magtheridon'], 1),
  },
  {
    expansion: 'The Burning Crusade',
    season: 1,
    raidName: 'Serpentshrine Cavern',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2007-03-29',
    bosses: bossesOf(
      [
        'Hydross the Unstable', 'The Lurker Below', 'Leotheras the Blind',
        'Fathom-Lord Karathress', 'Morogrim Tidewalker', 'Lady Vashj',
      ],
      6,
    ),
  },
  {
    expansion: 'The Burning Crusade',
    season: 1,
    raidName: 'Tempest Keep: The Eye',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2007-05-25',
    bosses: bossesOf(
      ['Al\'ar', 'Void Reaver', 'High Astromancer Solarian', 'Kael\'thas Sunstrider'],
      4,
    ),
  },
  {
    expansion: 'The Burning Crusade',
    season: 1,
    raidName: 'Black Temple',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2007-06-05',
    bosses: bossesOf(
      [
        'High Warlord Naj\'entus', 'Supremus', 'Shade of Akama', 'Teron Gorefiend',
        'Gurtogg Bloodboil', 'Reliquary of Souls', 'Mother Shahraz',
        'The Illidari Council', 'Illidan Stormrage',
      ],
      9,
    ),
  },
  {
    expansion: 'The Burning Crusade',
    season: 1,
    raidName: 'Mount Hyjal',
    worldFirstGuild: 'Nihilum',
    pullCount: 0,
    killDate: '2007-06-09',
    bosses: bossesOf(
      ['Rage Winterchill', 'Anetheron', 'Kaz\'rogal', 'Azgalor', 'Archimonde'],
      5,
    ),
  },
  {
    expansion: 'The Burning Crusade',
    season: 1,
    raidName: 'Sunwell Plateau',
    worldFirstGuild: 'SK Gaming',
    pullCount: 0,
    killDate: '2008-05-25',
    bosses: bossesOf(
      ['Kalecgos', 'Brutallus', 'Felmyst', 'Eredar Twins', 'M\'uru', 'Kil\'jaeden'],
      6,
    ),
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

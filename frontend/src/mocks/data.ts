import type {
  BossPullSeries,
  GuildProfile,
  GuildRaceEntry,
  HistoryBoss,
  HistoryTier,
  LiveTickerEvent,
  PvpBracket,
  PvpLadderEntry,
  PvpTier,
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
  // Wrath of the Lich King: World-First-Ergebnisse community-kuratiert nach Method
  // ("Wrath of the Lich King Raid History", method.gg/raid-history/wrath-of-the-lich-king).
  {
    expansion: 'Wrath of the Lich King',
    season: 1,
    raidName: 'Vault of Archavon',
    worldFirstGuild: '—',
    pullCount: 0,
    killDate: '',
    bosses: bossesOf(['Archavon the Stone Watcher'], 0),
  },
  {
    expansion: 'Wrath of the Lich King',
    season: 1,
    raidName: 'Naxxramas',
    worldFirstGuild: 'Ensidia',
    pullCount: 0,
    killDate: '2008-11-15',
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
  {
    expansion: 'Wrath of the Lich King',
    season: 1,
    raidName: 'The Obsidian Sanctum',
    worldFirstGuild: 'Ensidia',
    pullCount: 0,
    killDate: '2008-11-21',
    bosses: bossesOf(['Sartharion'], 1),
  },
  {
    expansion: 'Wrath of the Lich King',
    season: 1,
    raidName: 'The Eye of Eternity',
    worldFirstGuild: 'Ensidia',
    pullCount: 0,
    killDate: '2008-11-15',
    bosses: bossesOf(['Malygos'], 1),
  },
  {
    expansion: 'Wrath of the Lich King',
    season: 1,
    raidName: 'Ulduar',
    worldFirstGuild: 'Stars',
    pullCount: 0,
    killDate: '2009-07-07',
    bosses: bossesOf(
      [
        'Flame Leviathan', 'Ignis the Furnace Master', 'Razorscale', 'XT-002 Deconstructor',
        'Assembly of Iron', 'Kologarn', 'Auriaya', 'Hodir', 'Thorim', 'Freya',
        'Mimiron', 'General Vezax', 'Yogg-Saron',
      ],
      13,
    ),
  },
  {
    expansion: 'Wrath of the Lich King',
    season: 1,
    raidName: 'Trial of the Grand Crusader',
    worldFirstGuild: 'Paragon',
    pullCount: 0,
    killDate: '2009-09-07',
    bosses: bossesOf(
      ['Northrend Beasts', 'Lord Jaraxxus', 'Faction Champions', 'Val\'kyr Twins', 'Anub\'arak'],
      5,
    ),
  },
  {
    expansion: 'Wrath of the Lich King',
    season: 1,
    raidName: 'Icecrown Citadel',
    worldFirstGuild: 'Paragon',
    pullCount: 0,
    killDate: '2010-03-26',
    bosses: bossesOf(
      [
        'Lord Marrowgar', 'Lady Deathwhisper', 'Gunship Battle', 'Deathbringer Saurfang',
        'Festergut', 'Rotface', 'Professor Putricide', 'Blood Prince Council',
        'Blood-Queen Lana\'thel', 'Valithria Dreamwalker', 'Sindragosa', 'The Lich King',
      ],
      12,
    ),
  },
  {
    expansion: 'Wrath of the Lich King',
    season: 1,
    raidName: 'Ruby Sanctum',
    worldFirstGuild: 'Premonition',
    pullCount: 0,
    killDate: '2010-06-30',
    bosses: bossesOf(['Halion'], 1),
  },
  // Cataclysm: World-First-Ergebnisse community-kuratiert nach Method
  // ("Cataclysm Raid History", method.gg/raid-history/cataclysm).
  {
    expansion: 'Cataclysm',
    season: 1,
    raidName: 'Baradin Hold',
    worldFirstGuild: '—',
    pullCount: 0,
    killDate: '',
    bosses: bossesOf(['Argaloth'], 0),
  },
  {
    expansion: 'Cataclysm',
    season: 1,
    raidName: 'Blackwing Descent',
    worldFirstGuild: 'Paragon',
    pullCount: 0,
    killDate: '2011-01-09',
    bosses: bossesOf(
      ['Magmaw', 'Omnotron Defense System', 'Maloriak', 'Atramedes', 'Chimaeron', 'Nefarian'],
      6,
    ),
  },
  {
    expansion: 'Cataclysm',
    season: 1,
    raidName: 'The Bastion of Twilight',
    worldFirstGuild: 'Paragon',
    pullCount: 0,
    killDate: '2011-01-20',
    bosses: bossesOf(
      ['Halfus Wyrmbreaker', 'Valiona and Theralion', 'Ascendant Council', 'Cho\'gall', 'Sinestra'],
      5,
    ),
  },
  {
    expansion: 'Cataclysm',
    season: 1,
    raidName: 'Throne of the Four Winds',
    worldFirstGuild: 'Paragon',
    pullCount: 0,
    killDate: '2011-01-24',
    bosses: bossesOf(['Conclave of Wind', 'Al\'Akir'], 2),
  },
  {
    expansion: 'Cataclysm',
    season: 1,
    raidName: 'Firelands',
    worldFirstGuild: 'Paragon',
    pullCount: 0,
    killDate: '2011-07-19',
    bosses: bossesOf(
      [
        'Beth\'tilac', 'Lord Rhyolith', 'Alysrazor', 'Shannox',
        'Baleroc, the Gatekeeper', 'Majordomo Staghelm', 'Ragnaros',
      ],
      7,
    ),
  },
  {
    expansion: 'Cataclysm',
    season: 1,
    raidName: 'Dragon Soul',
    worldFirstGuild: 'KIN Raiders',
    pullCount: 0,
    killDate: '2011-12-20',
    bosses: bossesOf(
      [
        'Morchok', 'Warlord Zon\'ozz', 'Yor\'sahj the Unsleeping', 'Hagara the Stormbinder',
        'Ultraxion', 'Warmaster Blackhorn', 'Spine of Deathwing', 'Madness of Deathwing',
      ],
      8,
    ),
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
  // Mists of Pandaria bis Midnight: World-First-Ergebnisse community-kuratiert nach
  // Method (method.gg/raid-history/<addon>). Die Quelle dokumentiert hier jeden
  // Boss einzeln mit eigener Gilde/Datum — im Mock-Modus wird das auf den finalen
  // Boss der Boss-Rail verdichtet (siehe HistoryTile-Backend-DTO für die volle Kette).
  {
    expansion: 'Mists of Pandaria',
    season: 1,
    raidName: 'Mogu\'shan Vaults',
    worldFirstGuild: 'Method',
    pullCount: 0,
    killDate: '2012-10-12',
    bosses: bossesOf(
      [
        'The Stone Guard', 'Feng the Accursed', 'Gara\'jal the Spiritbinder',
        'The Spirit Kings', 'Elegon', 'Will of the Emperor',
      ],
      6,
    ),
  },
  {
    expansion: 'Mists of Pandaria',
    season: 1,
    raidName: 'Heart of Fear',
    worldFirstGuild: 'Blood Legion',
    pullCount: 0,
    killDate: '2012-11-11',
    bosses: bossesOf(
      [
        'Imperial Vizier Zor\'lok', 'Blade Lord Ta\'yak', 'Garalon',
        'Wind Lord Mel\'jarak', 'Amber-Shaper Un\'sok', 'Empress Shek\'zeer',
      ],
      6,
    ),
  },
  {
    expansion: 'Mists of Pandaria',
    season: 1,
    raidName: 'Terrace of Endless Spring',
    worldFirstGuild: 'Method',
    pullCount: 0,
    killDate: '2012-11-25',
    bosses: bossesOf(
      ['Protectors of the Endless', 'Tsulong', 'Lei Shi', 'Sha of Fear'],
      4,
    ),
  },
  {
    expansion: 'Mists of Pandaria',
    season: 1,
    raidName: 'Throne of Thunder',
    worldFirstGuild: 'Method',
    pullCount: 0,
    killDate: '2013-03-26',
    bosses: bossesOf(
      [
        'Jin\'rokh the Breaker', 'Horridon', 'Council of Elders', 'Tortos', 'Megaera',
        'Ji-Kun', 'Durumu the Forgotten', 'Primordius', 'Dark Animus', 'Iron Qon',
        'Twin Consorts', 'Ra-den', 'Lei Shen',
      ],
      13,
    ),
  },
  {
    expansion: 'Mists of Pandaria',
    season: 1,
    raidName: 'Siege of Orgrimmar',
    worldFirstGuild: 'Method',
    pullCount: 0,
    killDate: '2013-10-01',
    bosses: bossesOf(
      [
        'Immerseus', 'Fallen Protectors', 'Norushen', 'Sha of Pride', 'Galakras',
        'Iron Juggernaut', 'Kor\'kron Dark Shaman', 'General Nazgrim', 'Malkorok',
        'Spoils of Pandaria', 'Thok the Bloodthirsty', 'Siegecrafter Blackfuse',
        'Paragons of the Klaxxi', 'Garrosh Hellscream',
      ],
      14,
    ),
  },
  {
    expansion: 'Warlords of Draenor',
    season: 1,
    raidName: 'Highmaul',
    worldFirstGuild: 'Paragon',
    pullCount: 0,
    killDate: '2014-12-13',
    bosses: bossesOf(
      [
        'Kargath Bladefist', 'Brackenspore', 'Twin Ogron', 'Ko\'ragh',
        'Tectus', 'The Butcher', 'Imperator Mar\'gok',
      ],
      7,
    ),
  },
  {
    expansion: 'Warlords of Draenor',
    season: 1,
    raidName: 'Blackrock Foundry',
    worldFirstGuild: 'Method',
    pullCount: 0,
    killDate: '2015-02-20',
    bosses: bossesOf(
      [
        'Oregorger the Devourer', 'Gruul', 'Hans\'gar & Franzok', 'Beastlord Darmac',
        'Flamebender Ka\'graz', 'Operator Thogar', 'Kromog', 'The Iron Maidens', 'Blast Furnace', 'Blackhand',
      ],
      10,
    ),
  },
  {
    expansion: 'Warlords of Draenor',
    season: 1,
    raidName: 'Hellfire Citadel',
    worldFirstGuild: 'Method',
    pullCount: 0,
    killDate: '2015-07-16',
    bosses: bossesOf(
      [
        'Hellfire Assault', 'Iron Reaver', 'Kormrok', 'Hellfire High Council', 'Kilrogg Deadeye',
        'Gorefiend', 'Shadow-Lord Iskar', 'Fel Lord Zakuun', 'Socrethar the Eternal',
        'Tyrant Velhari', 'Xhul\'horac', 'Mannoroth', 'Archimonde',
      ],
      13,
    ),
  },
  {
    expansion: 'Legion',
    season: 1,
    raidName: 'The Emerald Nightmare',
    worldFirstGuild: 'Exorsus',
    pullCount: 0,
    killDate: '2016-09-29',
    bosses: bossesOf(
      ['Nythendra', 'Elerethe Renferal', 'Ursoc', 'Dragons of Nightmare', 'Il\'gynoth', 'Cenarius', 'Xavius'],
      7,
    ),
  },
  {
    expansion: 'Legion',
    season: 1,
    raidName: 'Trial of Valor',
    worldFirstGuild: 'Method',
    pullCount: 0,
    killDate: '2016-11-18',
    bosses: bossesOf(['Odyn', 'Guarm', 'Helya'], 3),
  },
  {
    expansion: 'Legion',
    season: 1,
    raidName: 'The Nighthold',
    worldFirstGuild: 'Exorsus',
    pullCount: 248,
    killDate: '2017-02-04',
    bosses: bossesOf(
      [
        'Skorpyron', 'Chronomatic Anomaly', 'Trilliax', 'Spellblade Aluriel', 'Tichondrius',
        'Krosus', 'High-Botanist Tel\'arn', 'Star-Augur Etraeus', 'Grand-Magistrix Elisande', 'Gul\'dan',
      ],
      10,
    ),
  },
  {
    expansion: 'Legion',
    season: 1,
    raidName: 'Tomb of Sargeras',
    worldFirstGuild: 'Method',
    pullCount: 654,
    killDate: '2017-07-16',
    bosses: bossesOf(
      [
        'Goroth', 'Demonic Inquisition', 'Harjatan', 'Sisters of the Moon', 'Mistress Sassz\'ine',
        'The Desolate Host', 'Maiden of Vigilance', 'Fallen Avatar', 'Kil\'jaeden',
      ],
      9,
    ),
  },
  {
    expansion: 'Legion',
    season: 1,
    raidName: 'Antorus, the Burning Throne',
    worldFirstGuild: 'Method',
    pullCount: 320,
    killDate: '2017-12-13',
    bosses: bossesOf(
      [
        'Garothi Worldbreaker', 'Felhounds of Sargeras', 'Portal Keeper Hasabel', 'Antoran High Command',
        'Eonar the Life-Binder', 'Imonar the Soulhunter', 'Kin\'garoth', 'Varimathras',
        'The Coven of Shivarra', 'Aggramar', 'Argus the Unmaker',
      ],
      11,
    ),
  },
  {
    expansion: 'Battle for Azeroth',
    season: 1,
    raidName: 'Uldir',
    worldFirstGuild: 'Method',
    pullCount: 285,
    killDate: '2018-09-19',
    bosses: bossesOf(
      [
        'Taloc the Corrupted', 'Mother', 'Zek\'voz, Herald of N\'zoth', 'Vectis',
        'Fetid Devourer', 'Zul, Reborn', 'Mythrax the Unraveler', 'G\'huun',
      ],
      8,
    ),
  },
  {
    expansion: 'Battle for Azeroth',
    season: 1,
    raidName: 'Battle of Dazar\'alor',
    worldFirstGuild: 'Method',
    pullCount: 346,
    killDate: '2019-02-05',
    bosses: bossesOf(
      [
        'Champion of the Light', 'Grong', 'Jadefire Masters', 'Opulence', 'Conclave of the Chosen',
        'King Rastakhan', 'High Tinker Mekkatorque', 'Stormwall Blockade', 'Lady Jaina Proudmoore',
      ],
      9,
    ),
  },
  {
    expansion: 'Battle for Azeroth',
    season: 1,
    raidName: 'Crucible of Storms',
    worldFirstGuild: 'Pieces',
    pullCount: 700,
    killDate: '2019-05-03',
    bosses: bossesOf(['The Restless Cabal', 'Uu\'nat, Harbinger of the Void'], 2),
  },
  {
    expansion: 'Battle for Azeroth',
    season: 1,
    raidName: 'The Eternal Palace',
    worldFirstGuild: 'Method',
    pullCount: 359,
    killDate: '2019-07-28',
    bosses: bossesOf(
      [
        'Abyssal Commander Sivara', 'Blackwater Behemoth', 'Radiance of Azshara', 'Lady Ashvane',
        'Orgozoa', 'The Queen\'s Court', 'Za\'qul', 'Queen Azshara',
      ],
      8,
    ),
  },
  {
    expansion: 'Battle for Azeroth',
    season: 1,
    raidName: 'Ny\'alotha, the Waking City',
    worldFirstGuild: 'Complexity Limit',
    pullCount: 270,
    killDate: '2020-02-06',
    bosses: bossesOf(
      [
        'Wrathion', 'Maut', 'The Prophet Skitra', 'Dark Inquisitor Xanesh', 'The Hivemind',
        'Shad\'har the Insatiable', 'Drest\'agath', 'Vexiona', 'Ra-den the Despoiled',
        'Il\'gynoth', 'Carapace of N\'Zoth', 'N\'Zoth the Corruptor',
      ],
      12,
    ),
  },
  {
    expansion: 'Shadowlands',
    season: 1,
    raidName: 'Castle Nathria',
    worldFirstGuild: 'Complexity Limit',
    pullCount: 0,
    killDate: '2020-12-23',
    bosses: bossesOf(
      [
        'Shriekwing', 'Altimor the Huntsman', 'Hungering Destroyer', 'Artificer Xy\'Mox',
        'Sun King\'s Salvation', 'Lady Inerva Darkvein', 'The Council of Blood', 'Sludgefist',
        'Stone Legion Generals', 'Sire Denathrius',
      ],
      10,
    ),
  },
  {
    expansion: 'Shadowlands',
    season: 1,
    raidName: 'Sanctum of Domination',
    worldFirstGuild: 'Echo',
    pullCount: 0,
    killDate: '2021-07-20',
    bosses: bossesOf(
      [
        'The Tarragrue', 'Eye of the Jailer', 'The Nine', 'Remnant of Ner\'zhul',
        'Soulrender Dormazain', 'Painsmith Raznal', 'Guardian of the First Ones',
        'Fatescribe Roh-Kalo', 'Kel\'Thuzad', 'Sylvanas Windrunner',
      ],
      10,
    ),
  },
  {
    expansion: 'Shadowlands',
    season: 1,
    raidName: 'Sepulcher of the First Ones',
    worldFirstGuild: 'Echo',
    pullCount: 0,
    killDate: '2022-03-26',
    bosses: bossesOf(
      [
        'Vigilant Guardian', 'Skolex, the Insatiable Ravener', 'Artificer Xy\'mox',
        'Dausegne, the Fallen Oracle', 'Prototype Pantheon', 'Lihuvim, Principal Architect',
        'Halondrus the Reclaimer', 'Anduin Wrynn', 'Lords of Dread', 'Rygelon', 'The Jailer, Zovaal',
      ],
      11,
    ),
  },
  {
    expansion: 'Dragonflight',
    season: 1,
    raidName: 'Vault of the Incarnates',
    worldFirstGuild: 'Echo',
    pullCount: 0,
    killDate: '2022-12-23',
    bosses: bossesOf(
      [
        'Eranog', 'Terros', 'The Primal Council', 'Sennarth, the Cold Breath',
        'Dathea, Ascended', 'Kurog Grimtotem', 'Broodkeeper Diurna', 'Raszageth the Storm-Eater',
      ],
      8,
    ),
  },
  {
    expansion: 'Dragonflight',
    season: 1,
    raidName: 'Aberrus, the Shadowed Crucible',
    worldFirstGuild: 'Liquid',
    pullCount: 0,
    killDate: '2023-05-15',
    bosses: bossesOf(
      [
        'Kazzara, the Hellforged', 'Assault of the Zaqali', 'The Amalgamation Chamber',
        'The Forgotten Experiments', 'Rashok, the Elder', 'The Vigilant Steward, Zskarn',
        'Magmorax', 'Echo of Neltharion', 'Scalecommander Sarkareth',
      ],
      9,
    ),
  },
  {
    expansion: 'Dragonflight',
    season: 1,
    raidName: 'Amirdrassil, the Dream\'s Hope',
    worldFirstGuild: 'Echo',
    pullCount: 0,
    killDate: '2023-11-26',
    bosses: bossesOf(
      [
        'Gnarlroot', 'Igira the Cruel', 'Volcoross', 'Council of Dreams',
        'Larodar, Keeper of the Flame', 'Nymue, Weaver of the Cycle', 'Smolderon',
        'Tindral Sageswift, Seer of the Flame', 'Fyrakk the Blazing',
      ],
      9,
    ),
  },
  {
    expansion: 'The War Within',
    season: 1,
    raidName: 'Nerub\'ar Palace',
    worldFirstGuild: 'Liquid',
    pullCount: 0,
    killDate: '2024-09-29',
    bosses: bossesOf(
      [
        'Ulgrax the Devourer', 'The Bloodbound Horror', 'Sikran, Captain of the Sureki', 'Rasha\'nan',
        'Broodtwister Ovi\'nax', 'Nexus-Princess Ky\'veza', 'The Silken Court', 'Queen Ansurek',
      ],
      8,
    ),
  },
  {
    expansion: 'The War Within',
    season: 1,
    raidName: 'Liberation of Undermine',
    worldFirstGuild: 'Liquid',
    pullCount: 0,
    killDate: '2025-03-16',
    bosses: bossesOf(
      [
        'Vexie and the Geargrinders', 'Cauldron of Carnage', 'Rik Reverb', 'Stix Bunkjunker',
        'Sprocketmonger Lockenstock', 'The One-Armed Bandit', 'Mug\'Zee, Heads of Security',
        'Chrome King Gallywix',
      ],
      8,
    ),
  },
  {
    expansion: 'The War Within',
    season: 1,
    raidName: 'Manaforge Omega',
    worldFirstGuild: 'Liquid',
    pullCount: 0,
    killDate: '2025-08-24',
    bosses: bossesOf(
      [
        'Plexus Sentinel', 'Soulbinder Naazindhri', 'Loom\'ithar', 'Forgeweaver Araz',
        'The Soul Hunters', 'Fractillus', 'Nexus-King Salhadaar', 'Dimensius, the All-Devouring',
      ],
      8,
    ),
  },
  {
    expansion: 'Midnight',
    season: 1,
    raidName: 'The Voidspire',
    worldFirstGuild: 'Liquid',
    pullCount: 0,
    killDate: '2026-03-27',
    bosses: bossesOf(
      [
        'Imperator Averzian', 'Vorasius', 'Fallen-King Salhadaar',
        'Vaelgor & Ezzorak', 'Lightblinded Vanguard', 'Crown of the Cosmos',
      ],
      6,
    ),
  },
  {
    expansion: 'Midnight',
    season: 1,
    raidName: 'The Dreamrift',
    worldFirstGuild: 'Liquid',
    pullCount: 0,
    killDate: '2026-03-25',
    bosses: bossesOf(['Chimaerus, the Undreamt God'], 1),
  },
  {
    expansion: 'Midnight',
    season: 1,
    raidName: 'March on Quel\'Danas',
    worldFirstGuild: 'Liquid',
    pullCount: 0,
    killDate: '2026-04-06',
    bosses: bossesOf(['Belo\'ren, Child of Al\'ar', 'Midnight Falls (L\'ura)'], 2),
  },
  {
    expansion: 'Midnight',
    season: 2,
    raidName: 'The Venomous Abyss',
    worldFirstGuild: '—',
    pullCount: 0,
    killDate: '',
    bosses: bossesOf(
      [
        'Nek\'zali the Soulcoiler', 'Entombed Sentinels', 'The Lost Explorers',
        'Vashnik the Malignant', 'Sszorak', 'The Twin Fangs', 'The Coiled Altar', 'Ula\'tek',
      ],
      0,
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

// Recap-Texte wie im Backend-GuildProfileSeeder recherchiert (Quellen u. a. Wikipedia,
// Blizzard Watch, teamliquid.com, method.gg, echoesports.gg) — hier für den Mock-Modus
// dupliziert, damit die UI ohne Backend die gleiche Erzählung zeigt.
export const mockGuildProfiles: GuildProfile[] = [
  {
    guild: { id: 'g1', name: 'Liquid', region: 'US', foundedYear: 2015 },
    status: 'active',
    bio:
      'Liquid Guild ist die direkte Fortsetzung von Complexity Limit (zuvor Limit, ' +
      'gegründet 2015): Im Januar 2022 übernahm die Esports-Organisation Team Liquid ' +
      'die Gilde und gliederte sie als eigene MMO-Sparte ein. Seit Sepulcher of the ' +
      'First Ones zählt Liquid durchgehend zur Weltspitze und hat unter anderem ' +
      "Nerub'ar Palace, Liberation of Undermine, Manaforge Omega und die ersten beiden " +
      'Midnight-Raids als Erste bezwungen.',
    links: {
      twitch: 'https://www.twitch.tv/teamliquid',
      youTube: 'https://www.youtube.com/@TeamLiquidMMO',
      twitter: 'https://x.com/LiquidGuild',
      website: 'https://teamliquid.com/games/wow',
    },
    history: [
      {
        expansion: 'Midnight',
        raidName: "March on Quel'Danas",
        bossName: "Midnight Falls (L'ura)",
        killDate: '2026-04-06',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/midnight',
      },
      {
        expansion: 'The War Within',
        raidName: 'Manaforge Omega',
        bossName: 'Dimensius, the All-Devouring',
        killDate: '2025-08-24',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/the-war-within',
      },
      {
        expansion: 'The War Within',
        raidName: "Nerub'ar Palace",
        bossName: 'Queen Ansurek',
        killDate: '2024-09-29',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/the-war-within',
      },
      {
        expansion: 'Dragonflight',
        raidName: 'Aberrus, the Shadowed Crucible',
        bossName: 'Scalecommander Sarkareth',
        killDate: '2023-05-15',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/dragonflight',
      },
      {
        expansion: 'Shadowlands',
        raidName: 'Sepulcher of the First Ones',
        bossName: 'The Jailer, Zovaal',
        killDate: '2022-03-26',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/shadowlands',
      },
    ],
  },
  {
    guild: { id: 'g2', name: 'Echo', region: 'EU', foundedYear: 2020 },
    status: 'active',
    bio:
      'Echo entstand im Juli 2020 aus dem größten Teil des ehemaligen Method-Raid-Kaders, ' +
      'kurz nachdem die Method-Organisation durch einen Missbrauchsskandal ' +
      'auseinandergebrochen war. Seit Shadowlands zählt Echo durchgehend zur absoluten ' +
      'Weltspitze und hat unter anderem Sepulcher of the First Ones, Vault of the ' +
      "Incarnates und Amirdrassil, the Dream's Hope als Erste bezwungen.",
    links: {
      twitch: 'https://www.twitch.tv/echo_esports',
      twitter: 'https://twitter.com/EchoGuild',
      website: 'https://www.echoesports.gg',
    },
    history: [
      {
        expansion: 'The War Within',
        raidName: 'Liberation of Undermine',
        bossName: 'Sprocketmonger Lockenstock',
        killDate: '2025-03-11',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/the-war-within',
      },
      {
        expansion: 'Dragonflight',
        raidName: "Amirdrassil, the Dream's Hope",
        bossName: 'Fyrakk the Blazing',
        killDate: '2023-11-26',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/dragonflight',
      },
      {
        expansion: 'Dragonflight',
        raidName: 'Vault of the Incarnates',
        bossName: 'Raszageth the Storm-Eater',
        killDate: '2022-12-23',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/dragonflight',
      },
    ],
  },
  {
    guild: { id: 'g3', name: 'Complexity Limit', region: 'US' },
    status: 'disbanded',
    disbandedYear: 2022,
    bio:
      'Aus der im Oktober 2019 geschlossenen Partnerschaft von Limit mit der Organisation ' +
      'Complexity hervorgegangen, erreichte Complexity Limit im Februar 2020 mit ' +
      "Ny'alotha, the Waking City erstmals den Rang der weltbesten Gilde. Im Januar 2022 " +
      'wurde die Gilde von Team Liquid übernommen und trat fortan als Liquid Guild an.',
    links: {},
    history: [
      {
        expansion: 'Battle for Azeroth',
        raidName: "Ny'alotha, the Waking City",
        bossName: "N'Zoth the Corruptor",
        killDate: '2020-02-06',
        pullCount: 0,
        sourceUrl: 'https://www.method.gg/raid-history/battle-for-azeroth',
      },
    ],
  },
  {
    // Keine recherchierte Bio vorhanden — zeigt den Fallback-Zustand der UI.
    guild: { id: 'g4', name: 'BDGG', region: 'EU' },
    status: 'unknown',
    links: {},
    history: [],
  },
];

// Beta: Rating-Leiter-Dashboard fürs PvP-Pendant zu "Race to World First". Es gibt
// (noch) keine Blizzard-Battle.net-API-Anbindung — daher rein fiktive Team-/
// Spielernamen statt erfundener Ratings für echte Personen (gespiegelt aus dem
// Backend-PvpDemoSeeder, damit Mock- und Echtmodus gleich aussehen).
function tierFor(rating: number): PvpTier {
  if (rating >= 2400) return 'Gladiator';
  if (rating >= 2100) return 'Elite';
  if (rating >= 1800) return 'Duelist';
  if (rating >= 1600) return 'Rival';
  if (rating >= 1400) return 'Challenger';
  return 'Combatant';
}

function ladderOf(
  bracket: PvpBracket,
  teams: { name: string; region: string; rating: number; players: string[] }[],
): PvpLadderEntry[] {
  return teams
    .slice()
    .sort((a, b) => b.rating - a.rating)
    .map((t, i) => ({
      rank: i + 1,
      id: `${bracket}-${i}`,
      name: t.name,
      region: t.region,
      bracket,
      rating: t.rating,
      tier: tierFor(t.rating),
      players: t.players,
      updatedAt: new Date().toISOString(),
    }));
}

export const mockPvpLadder: Record<PvpBracket, PvpLadderEntry[]> = {
  '3v3': ladderOf('3v3', [
    { name: 'Ember Vanguard', region: 'EU', rating: 2687, players: ['Nightglass', 'Suncaller', 'Vraskor'] },
    { name: 'Frostcoil Trio', region: 'EU', rating: 2541, players: ['Ashenwake', 'Coldmourne', 'Thundervex'] },
    { name: 'Sable Wardens', region: 'US', rating: 2488, players: ['Grimhollow', 'Ravenshade', 'Duskwarden'] },
    { name: 'Voidbound Three', region: 'US', rating: 2312, players: ['Nyxaria', 'Shadowmere', 'Emberfall'] },
    { name: 'Ironclad Trinity', region: 'TW', rating: 2156, players: ['Steelrend', 'Warglory', 'Ironvow'] },
    { name: 'Wraithcall', region: 'KR', rating: 2034, players: ['Hexbane', 'Soulrend', 'Netherquill'] },
    { name: 'Stormforged', region: 'EU', rating: 1876, players: ['Galewind', 'Tempestra', 'Boltcaster'] },
    { name: 'Dawnwatch', region: 'US', rating: 1622, players: ['Sunveil', 'Lightbringer', 'Aurorafen'] },
  ]),
  '2v2': ladderOf('2v2', [
    { name: 'Twin Fangs', region: 'EU', rating: 2521, players: ['Venomstrike', 'Coldbite'] },
    { name: 'Ashen Duo', region: 'US', rating: 2398, players: ['Cinderwake', 'Grimfall'] },
    { name: 'Skyward Pair', region: 'EU', rating: 2201, players: ['Windrider', 'Stormsong'] },
    { name: 'Nightfall Two', region: 'TW', rating: 2065, players: ['Duskbringer', 'Moonshade'] },
    { name: 'Ironbound', region: 'KR', rating: 1889, players: ['Anvilheart', 'Forgewrath'] },
    { name: 'Emberkin', region: 'US', rating: 1655, players: ['Blazewing', 'Pyrestep'] },
  ]),
  rbg: ladderOf('rbg', [
    { name: 'Crimson Battalion', region: 'EU', rating: 2477, players: Array.from({ length: 10 }, (_, i) => `CrimsonWarden${i + 1}`) },
    { name: 'Northwatch Legion', region: 'US', rating: 2298, players: Array.from({ length: 10 }, (_, i) => `NorthwatchWarden${i + 1}`) },
    { name: 'Ashfall Regiment', region: 'EU', rating: 2109, players: Array.from({ length: 10 }, (_, i) => `AshfallWarden${i + 1}`) },
    { name: 'Silverpine Guard', region: 'US', rating: 1934, players: Array.from({ length: 10 }, (_, i) => `SilverpineWarden${i + 1}`) },
    { name: 'Stormcrest Company', region: 'TW', rating: 1748, players: Array.from({ length: 10 }, (_, i) => `StormcrestWarden${i + 1}`) },
  ]),
  'solo-shuffle': ladderOf('solo-shuffle', [
    { name: 'Vex the Unseen', region: 'EU', rating: 2589, players: ['Vex the Unseen'] },
    { name: 'Korrath Ashblade', region: 'US', rating: 2444, players: ['Korrath Ashblade'] },
    { name: 'Mirelle Duskthorn', region: 'EU', rating: 2287, players: ['Mirelle Duskthorn'] },
    { name: 'Baelor Stormrend', region: 'US', rating: 2103, players: ['Baelor Stormrend'] },
    { name: 'Syvane Nightglow', region: 'KR', rating: 1912, players: ['Syvane Nightglow'] },
    { name: 'Tharion Wolfsbane', region: 'TW', rating: 1701, players: ['Tharion Wolfsbane'] },
  ]),
};

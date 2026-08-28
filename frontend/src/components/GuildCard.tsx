import { Link } from 'react-router-dom';
import type { GuildRaceEntry } from '../types';
import { BossRail } from './BossRail';

function formatNumber(n: number) {
  return new Intl.NumberFormat('de-DE').format(n);
}

function timeAgo(iso?: string) {
  if (!iso) return '—';
  const diffMs = Date.now() - new Date(iso).getTime();
  const minutes = Math.floor(diffMs / 60_000);
  if (minutes < 60) return `vor ${minutes} min`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `vor ${hours} h`;
  return `vor ${Math.floor(hours / 24)} d`;
}

export function GuildCard({ entry }: { entry: GuildRaceEntry }) {
  return (
    <Link
      to={`/guilds/${entry.guild.id}`}
      className="block rounded-[10px] border border-border bg-card p-4 transition-colors hover:border-turquoise/50"
    >
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <span className="font-mono-num text-lg text-text-muted">#{entry.rank}</span>
          <div>
            <div className="font-headline text-xl leading-none text-text">
              {entry.guild.name}
            </div>
            <div className="eyebrow text-[11px] text-text-muted">{entry.guild.region}</div>
          </div>
        </div>
        <div className="text-right">
          <div className="font-mono-num text-sm text-gold-light">
            {entry.bossesKilled}/{entry.bosses.length} Bosse
          </div>
          <div className="font-mono-num text-xs text-text-muted">
            {formatNumber(entry.totalPulls)} Pulls
          </div>
        </div>
      </div>

      <div className="mt-4">
        <BossRail bosses={entry.bosses} />
      </div>

      <div className="mt-3 text-xs text-text-muted">
        Letzter Kill: <span className="text-text">{timeAgo(entry.lastKillAt)}</span>
      </div>
    </Link>
  );
}

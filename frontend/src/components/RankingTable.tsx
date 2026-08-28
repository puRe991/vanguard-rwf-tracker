import { Link } from 'react-router-dom';
import type { GuildRaceEntry } from '../types';

function formatNumber(n: number) {
  return new Intl.NumberFormat('de-DE').format(n);
}

export function RankingTable({ entries }: { entries: GuildRaceEntry[] }) {
  return (
    <div className="overflow-hidden rounded-[10px] border border-border bg-card">
      <table className="w-full text-left text-sm">
        <thead>
          <tr className="border-b border-border text-text-muted">
            <th className="eyebrow px-4 py-3 text-[11px] font-normal">#</th>
            <th className="eyebrow px-4 py-3 text-[11px] font-normal">Gilde</th>
            <th className="eyebrow px-4 py-3 text-[11px] font-normal">Region</th>
            <th className="eyebrow px-4 py-3 text-[11px] font-normal">Bosse</th>
            <th className="eyebrow px-4 py-3 text-[11px] font-normal">Pulls</th>
          </tr>
        </thead>
        <tbody>
          {entries.map((entry) => (
            <tr key={entry.guild.id} className="border-b border-border last:border-0 hover:bg-obsidian/40">
              <td className="font-mono-num px-4 py-3 text-text-muted">{entry.rank}</td>
              <td className="px-4 py-3">
                <Link to={`/guilds/${entry.guild.id}`} className="text-text hover:text-turquoise">
                  {entry.guild.name}
                </Link>
              </td>
              <td className="px-4 py-3 text-text-muted">{entry.guild.region}</td>
              <td className="font-mono-num px-4 py-3 text-gold-light">
                {entry.bossesKilled}/{entry.bosses.length}
              </td>
              <td className="font-mono-num px-4 py-3 text-text-muted">
                {formatNumber(entry.totalPulls)}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

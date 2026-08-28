import { useParams } from 'react-router-dom';
import { useGuild } from '../hooks/useGuild';
import { BossRail } from '../components/BossRail';

export function GuildProfile() {
  const { id } = useParams<{ id: string }>();
  const { data: entry, isLoading } = useGuild(id);

  if (isLoading) return <p className="text-text-muted">Lade Gilden-Profil…</p>;
  if (!entry) return <p className="text-text-muted">Gilde nicht gefunden.</p>;

  return (
    <div>
      <p className="eyebrow text-xs text-turquoise">Gilden-Profil</p>
      <h1 className="font-headline text-4xl">{entry.guild.name}</h1>
      <p className="mb-6 text-text-muted">{entry.guild.region}</p>

      <div className="rounded-[10px] border border-border bg-card p-4">
        <h2 className="eyebrow mb-3 text-xs text-text-muted">Aktueller Fortschritt</h2>
        <BossRail bosses={entry.bosses} />
        <div className="mt-4 flex gap-6 font-mono-num text-sm">
          <span className="text-gold-light">
            {entry.bossesKilled}/{entry.bosses.length} Bosse
          </span>
          <span className="text-text-muted">{entry.totalPulls} Pulls gesamt</span>
        </div>
      </div>
    </div>
  );
}

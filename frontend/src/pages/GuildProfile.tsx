import { useMemo, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useGuild, useGuildProfile } from '../hooks/useGuild';
import { BossRail } from '../components/BossRail';
import type { GuildHistoryKill, GuildLifecycleStatus } from '../types';

const STATUS_LABEL: Record<GuildLifecycleStatus, string> = {
  active: 'Aktiv',
  disbanded: 'Aufgelöst',
  retired: 'Nicht mehr im Spitzenfeld',
  unknown: 'Status unbekannt',
};

const STATUS_COLOR: Record<GuildLifecycleStatus, string> = {
  active: 'border-turquoise text-turquoise',
  disbanded: 'border-ember text-ember-light',
  retired: 'border-gold text-gold-light',
  unknown: 'border-border text-text-muted',
};

function StatusBadge({ status, disbandedYear }: { status: GuildLifecycleStatus; disbandedYear?: number }) {
  const label = STATUS_LABEL[status];
  const suffix = disbandedYear && (status === 'disbanded' || status === 'retired') ? ` (${disbandedYear})` : '';
  return (
    <span className={`eyebrow rounded-full border px-2 py-0.5 text-[11px] ${STATUS_COLOR[status]}`}>
      {label}
      {suffix}
    </span>
  );
}

function LinkRow({ links }: { links: { twitch?: string; youTube?: string; twitter?: string; website?: string } }) {
  const entries: { label: string; url: string }[] = [
    links.twitch && { label: 'Twitch', url: links.twitch },
    links.youTube && { label: 'YouTube', url: links.youTube },
    links.twitter && { label: 'Twitter/X', url: links.twitter },
    links.website && { label: 'Website', url: links.website },
  ].filter((e): e is { label: string; url: string } => Boolean(e));

  if (entries.length === 0) return null;

  return (
    <div className="mt-3 flex flex-wrap gap-3 text-xs">
      {entries.map((e) => (
        <a
          key={e.label}
          href={e.url}
          target="_blank"
          rel="noreferrer"
          className="rounded-full border border-border px-3 py-1 text-text-muted hover:border-turquoise hover:text-turquoise"
        >
          {e.label}
        </a>
      ))}
    </div>
  );
}

function RosterToggle({ kill }: { kill: GuildHistoryKill }) {
  const [open, setOpen] = useState(false);
  if (!kill.roster || kill.roster.length === 0) return null;

  return (
    <div className="mt-1">
      <button
        onClick={() => setOpen((v) => !v)}
        className="eyebrow flex items-center gap-1 text-[11px] text-text-muted hover:text-turquoise"
      >
        Roster ({kill.roster.length})
        <span className={`transition-transform ${open ? 'rotate-180' : ''}`} aria-hidden>
          ▾
        </span>
      </button>
      {open && (
        <ul className="mt-2 flex flex-wrap gap-2">
          {kill.roster.map((name) => (
            <li
              key={name}
              className="rounded-full border border-border bg-obsidian px-2 py-0.5 font-mono-num text-[11px] text-text"
            >
              {name}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

function HistoryList({ history }: { history: GuildHistoryKill[] }) {
  const [openExpansion, setOpenExpansion] = useState<string | null>(null);

  const grouped = useMemo(() => {
    const byExpansion = new Map<string, GuildHistoryKill[]>();
    for (const kill of history) {
      const list = byExpansion.get(kill.expansion) ?? [];
      list.push(kill);
      byExpansion.set(kill.expansion, list);
    }
    return Array.from(byExpansion.entries());
  }, [history]);

  if (grouped.length === 0) {
    return <p className="text-sm text-text-muted">Noch keine dokumentierten Kills für diese Gilde.</p>;
  }

  return (
    <div className="flex flex-col gap-2">
      {grouped.map(([expansion, kills], i) => {
        const open = openExpansion === expansion || (openExpansion === null && i === 0);
        return (
          <div key={expansion} className="rounded-[10px] border border-border bg-card">
            <button
              onClick={() => setOpenExpansion(open ? '' : expansion)}
              className="flex w-full items-center justify-between px-4 py-3 text-left"
            >
              <div className="eyebrow text-xs text-turquoise">
                {expansion} <span className="text-text-muted">· {kills.length} Kills</span>
              </div>
              <span className={`text-text-muted transition-transform ${open ? 'rotate-180' : ''}`} aria-hidden>
                ▾
              </span>
            </button>
            {open && (
              <ul className="flex flex-col gap-2 border-t border-border p-4">
                {kills.map((kill) => (
                  <li key={`${kill.raidName}-${kill.bossName}`} className="text-sm">
                    <div className="flex items-center justify-between gap-3">
                      <div>
                        <span className="text-text">{kill.bossName}</span>
                        <span className="text-text-muted"> — {kill.raidName}</span>
                      </div>
                      <div className="flex items-center gap-3 whitespace-nowrap">
                        <span className="font-mono-num text-xs text-text-muted">
                          {new Date(kill.killDate).toLocaleDateString('de-DE')}
                        </span>
                        {kill.sourceUrl && (
                          <a
                            href={kill.sourceUrl}
                            target="_blank"
                            rel="noreferrer"
                            className="text-xs text-gold-light hover:underline"
                          >
                            Beleg/Video
                          </a>
                        )}
                      </div>
                    </div>
                    <RosterToggle kill={kill} />
                  </li>
                ))}
              </ul>
            )}
          </div>
        );
      })}
    </div>
  );
}

export function GuildProfile() {
  const { id } = useParams<{ id: string }>();
  const { data: profile, isLoading: profileLoading } = useGuildProfile(id);
  const { data: currentEntry } = useGuild(id);

  if (profileLoading) return <p className="text-text-muted">Lade Gilden-Profil…</p>;
  if (!profile) return <p className="text-text-muted">Gilde nicht gefunden.</p>;

  return (
    <div>
      <p className="eyebrow text-xs text-turquoise">Gilden-Profil</p>
      <div className="mb-1 flex flex-wrap items-center gap-3">
        <h1 className="font-headline text-4xl">{profile.guild.name}</h1>
        <StatusBadge status={profile.status} disbandedYear={profile.disbandedYear} />
      </div>
      <p className="mb-4 text-text-muted">
        {profile.guild.region}
        {profile.guild.foundedYear ? ` · gegründet ${profile.guild.foundedYear}` : ''}
      </p>

      {profile.bio && (
        <div className="mb-6 rounded-[10px] border border-border bg-card p-4">
          <h2 className="eyebrow mb-2 text-xs text-text-muted">Recap</h2>
          <p className="max-w-3xl text-sm leading-relaxed text-text">{profile.bio}</p>
          <LinkRow links={profile.links} />
        </div>
      )}

      {currentEntry && (
        <div className="mb-6 rounded-[10px] border border-border bg-card p-4">
          <h2 className="eyebrow mb-3 text-xs text-text-muted">Aktueller Fortschritt</h2>
          <BossRail bosses={currentEntry.bosses} />
          <div className="mt-4 flex gap-6 font-mono-num text-sm">
            <span className="text-gold-light">
              {currentEntry.bossesKilled}/{currentEntry.bosses.length} Bosse
            </span>
            <span className="text-text-muted">{currentEntry.totalPulls} Pulls gesamt</span>
          </div>
        </div>
      )}

      <h2 className="eyebrow mb-3 text-xs text-text-muted">
        Kill-Historie{' '}
        <span className="text-text-muted">
          ({profile.history.length} World-First-{profile.history.length === 1 ? 'Kill' : 'Kills'})
        </span>
      </h2>
      <HistoryList history={profile.history} />
    </div>
  );
}

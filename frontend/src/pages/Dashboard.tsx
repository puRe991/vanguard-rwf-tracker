import { useLiveRace, useLiveTicker } from '../hooks/useLiveRace';
import { GuildCard } from '../components/GuildCard';
import { LiveTicker } from '../components/LiveTicker';

export function Dashboard() {
  const { data: entries, isLoading, isError } = useLiveRace();
  const { events } = useLiveTicker();

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <p className="eyebrow text-xs text-ember-light">Aktuelle Race</p>
          <h1 className="font-headline text-4xl">Nerub-ar Sanctum — Mythic</h1>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-[1fr_320px]">
        <div className="flex flex-col gap-3">
          {isLoading && <p className="text-text-muted">Lade Race-Daten…</p>}
          {isError && <p className="text-ember-light">Race-Daten konnten nicht geladen werden.</p>}
          {entries?.map((entry) => (
            <GuildCard key={entry.guild.id} entry={entry} />
          ))}
        </div>
        <div>
          <LiveTicker events={events} />
        </div>
      </div>
    </div>
  );
}

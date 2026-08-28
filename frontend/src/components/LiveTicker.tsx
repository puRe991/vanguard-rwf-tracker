import type { LiveTickerEvent } from '../types';

function timeAgo(iso: string) {
  const diffMs = Date.now() - new Date(iso).getTime();
  const minutes = Math.floor(diffMs / 60_000);
  if (minutes < 1) return 'gerade eben';
  if (minutes < 60) return `vor ${minutes} min`;
  const hours = Math.floor(minutes / 60);
  return `vor ${hours} h`;
}

const kindColor: Record<LiveTickerEvent['kind'], string> = {
  kill: 'bg-gold',
  'pull-milestone': 'bg-turquoise',
  'live-start': 'bg-ember',
};

export function LiveTicker({ events }: { events: LiveTickerEvent[] }) {
  return (
    <div className="rounded-[10px] border border-border bg-card p-4">
      <div className="mb-3 flex items-center gap-2">
        <span className="relative flex h-2 w-2">
          <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-turquoise opacity-75" />
          <span className="relative inline-flex h-2 w-2 rounded-full bg-turquoise" />
        </span>
        <h2 className="eyebrow text-xs text-turquoise">Live-Ticker</h2>
      </div>
      <ul className="flex flex-col gap-3">
        {events.map((event) => (
          <li key={event.id} className="flex items-start gap-2 text-sm">
            <span className={`mt-1.5 h-1.5 w-1.5 flex-shrink-0 rounded-full ${kindColor[event.kind]}`} />
            <div>
              <div className="text-text">{event.message}</div>
              <div className="font-mono-num text-xs text-text-muted">{timeAgo(event.timestamp)}</div>
            </div>
          </li>
        ))}
        {events.length === 0 && (
          <li className="text-sm text-text-muted">Noch keine Ereignisse.</li>
        )}
      </ul>
    </div>
  );
}

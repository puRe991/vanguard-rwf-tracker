import { useHubConnectionStatus } from '../hooks/useHubConnectionStatus';

const CONFIG = {
  connected: { label: 'Live', dot: 'bg-turquoise', text: 'text-turquoise', pulse: true },
  reconnecting: { label: 'Verbindung wird aufgebaut…', dot: 'bg-gold', text: 'text-gold-light', pulse: true },
  disconnected: { label: 'Getrennt', dot: 'bg-ember', text: 'text-ember-light', pulse: false },
  connecting: { label: 'Verbindung wird aufgebaut…', dot: 'bg-gold', text: 'text-gold-light', pulse: true },
  mock: { label: 'Demo-Daten', dot: 'bg-text-muted', text: 'text-text-muted', pulse: false },
} as const;

export function LiveStatusBadge() {
  const status = useHubConnectionStatus();
  const { label, dot, text, pulse } = CONFIG[status];

  return (
    <div className={`eyebrow flex items-center gap-1.5 text-[11px] ${text}`} title="RaceHub-Verbindungsstatus">
      <span className="relative flex h-1.5 w-1.5">
        {pulse && (
          <span className={`absolute inline-flex h-full w-full animate-ping rounded-full opacity-75 ${dot}`} />
        )}
        <span className={`relative inline-flex h-1.5 w-1.5 rounded-full ${dot}`} />
      </span>
      {label}
    </div>
  );
}

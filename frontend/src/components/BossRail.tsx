import type { BossProgress } from '../types';

function formatPulls(n: number) {
  return new Intl.NumberFormat('de-DE').format(n);
}

export function BossRail({ bosses }: { bosses: BossProgress[] }) {
  return (
    <div className="flex items-center gap-1.5">
      {bosses.map((boss) => (
        <div key={boss.id} className="group relative flex flex-col items-center">
          <div
            className={[
              'flex h-7 w-7 items-center justify-center rounded-full border text-xs',
              boss.status === 'killed' &&
                'border-gold bg-gold/15 text-gold-light',
              boss.status === 'active' &&
                'border-ember bg-ember/15 text-ember-light shadow-[0_0_10px_rgba(193,67,43,0.55)]',
              boss.status === 'locked' &&
                'border-border bg-card text-text-muted',
            ]
              .filter(Boolean)
              .join(' ')}
            title={boss.name}
          >
            {boss.status === 'killed' ? '✓' : boss.order + 1}
          </div>
          <div className="pointer-events-none absolute top-9 z-10 hidden whitespace-nowrap rounded-md border border-border bg-card px-2 py-1 text-xs text-text shadow-lg group-hover:block">
            <div className="font-medium">{boss.name}</div>
            {boss.status === 'killed' && (
              <div className="font-mono-num text-gold-light">Erledigt</div>
            )}
            {boss.status === 'active' && (
              <div className="font-mono-num text-ember-light">
                Pull {formatPulls(boss.pullCount ?? 0)}
              </div>
            )}
            {boss.status === 'locked' && <div className="text-text-muted">Gesperrt</div>}
          </div>
        </div>
      ))}
    </div>
  );
}

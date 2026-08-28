import type { PvpTier } from '../types';

const TIERS: PvpTier[] = ['Combatant', 'Challenger', 'Rival', 'Duelist', 'Elite', 'Gladiator'];

/** Tier-Fortschrittsanzeige fürs PvP-Beta — bewusst im selben visuellen Vokabular
 * wie BossRail (Knoten, Gold = erreicht, Ember-Glow = aktueller Tier). */
export function RatingRail({ tier }: { tier: PvpTier }) {
  const currentIndex = TIERS.indexOf(tier);

  return (
    <div className="flex items-center gap-1.5">
      {TIERS.map((t, i) => {
        const status = i < currentIndex ? 'reached' : i === currentIndex ? 'current' : 'locked';
        return (
          <div key={t} className="group relative flex flex-col items-center">
            <div
              className={[
                'flex h-6 w-6 items-center justify-center rounded-full border text-[10px]',
                status === 'reached' && 'border-gold bg-gold/15 text-gold-light',
                status === 'current' &&
                  'border-ember bg-ember/15 text-ember-light shadow-[0_0_10px_rgba(193,67,43,0.55)]',
                status === 'locked' && 'border-border bg-card text-text-muted',
              ]
                .filter(Boolean)
                .join(' ')}
              title={t}
            >
              {status === 'reached' ? '✓' : i + 1}
            </div>
            <div className="pointer-events-none absolute top-8 z-10 hidden whitespace-nowrap rounded-md border border-border bg-card px-2 py-1 text-xs text-text shadow-lg group-hover:block">
              {t}
            </div>
          </div>
        );
      })}
    </div>
  );
}

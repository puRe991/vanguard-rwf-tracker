import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useHistory } from '../hooks/useHistory';
import type { HistoryBoss, HistoryTier } from '../types';

const EXPANSIONS = [
  'Midnight',
  'The War Within',
  'Dragonflight',
  'Shadowlands',
  'Battle for Azeroth',
  'Legion',
  'Warlords of Draenor',
  'Mists of Pandaria',
  'Cataclysm',
  'Wrath of the Lich King',
  'The Burning Crusade',
  'Classic',
];

function HistoryBossRail({ bosses }: { bosses: HistoryBoss[] }) {
  if (bosses.length === 0) return null;

  return (
    <div className="mt-3 flex flex-wrap gap-1.5">
      {bosses.map((boss) => (
        <div
          key={boss.order}
          title={boss.name}
          className={[
            'flex h-6 items-center rounded-full border px-2 text-[11px]',
            boss.killed
              ? 'border-gold bg-gold/15 text-gold-light'
              : 'border-border bg-obsidian/40 text-text-muted',
          ].join(' ')}
        >
          {boss.name}
        </div>
      ))}
    </div>
  );
}

function TierCard({ tier }: { tier: HistoryTier }) {
  return (
    <div className="rounded-[10px] border border-border bg-card p-4">
      <div className="flex items-center justify-between">
        <div>
          <div className="eyebrow text-[11px] text-text-muted">Season {tier.season}</div>
          <div className="font-headline text-xl">{tier.raidName}</div>
        </div>
        <div className="text-right">
          {tier.worldFirstGuild === '—' ? (
            <Link to="/submit" className="text-sm text-ember-light hover:underline">
              Noch nicht dokumentiert — beitragen
            </Link>
          ) : (
            <>
              <div className="text-gold-light">{tier.worldFirstGuild}</div>
              <div className="font-mono-num text-xs text-text-muted">
                {tier.pullCount > 0 ? `${tier.pullCount} Pulls · ` : ''}
                {new Date(tier.killDate).toLocaleDateString('de-DE')}
              </div>
            </>
          )}
        </div>
      </div>

      <HistoryBossRail bosses={tier.bosses} />
    </div>
  );
}

function ExpansionSection({
  expansion,
  tiers,
  open,
  onToggle,
}: {
  expansion: string;
  tiers: HistoryTier[];
  open: boolean;
  onToggle: () => void;
}) {
  return (
    <div className="rounded-[10px] border border-border bg-card">
      <button
        onClick={onToggle}
        className="flex w-full items-center justify-between px-4 py-3 text-left"
      >
        <div>
          <div className="eyebrow text-[11px] text-turquoise">{expansion}</div>
          <div className="font-headline text-lg">{tiers.length} Raid-Tiers</div>
        </div>
        <span
          className={`text-text-muted transition-transform ${open ? 'rotate-180' : ''}`}
          aria-hidden
        >
          ▾
        </span>
      </button>

      {open && (
        <div className="flex flex-col gap-3 border-t border-border p-4">
          {tiers.map((tier) => (
            <TierCard key={`${tier.expansion}-${tier.season}-${tier.raidName}`} tier={tier} />
          ))}
        </div>
      )}
    </div>
  );
}

export function History() {
  const [expansion, setExpansion] = useState<string | undefined>();
  const { data: tiers, isLoading } = useHistory({ expansion });
  const [openSet, setOpenSet] = useState<Set<string>>(() => new Set([EXPANSIONS[0]]));

  const grouped = useMemo(() => {
    const byExpansion = new Map<string, HistoryTier[]>();
    for (const tier of tiers ?? []) {
      const list = byExpansion.get(tier.expansion) ?? [];
      list.push(tier);
      byExpansion.set(tier.expansion, list);
    }
    // In fester, chronologisch absteigender Reihenfolge rendern statt in
    // Datenreihenfolge, damit die Sektionen immer gleich sortiert sind.
    return EXPANSIONS.filter((exp) => byExpansion.has(exp)).map((exp) => ({
      expansion: exp,
      tiers: byExpansion.get(exp)!,
    }));
  }, [tiers]);

  function toggle(exp: string) {
    setOpenSet((prev) => {
      const next = new Set(prev);
      if (next.has(exp)) next.delete(exp);
      else next.add(exp);
      return next;
    });
  }

  return (
    <div>
      <p className="eyebrow text-xs text-turquoise">Historie</p>
      <h1 className="font-headline mb-6 text-4xl">Race to World First — Zeitleiste</h1>

      <div className="mb-6 flex flex-wrap gap-2">
        <button
          onClick={() => setExpansion(undefined)}
          className={`rounded-full border px-3 py-1 text-xs eyebrow ${
            !expansion ? 'border-turquoise text-turquoise' : 'border-border text-text-muted'
          }`}
        >
          Alle
        </button>
        {EXPANSIONS.map((exp) => (
          <button
            key={exp}
            onClick={() => {
              setExpansion(exp);
              setOpenSet(new Set([exp]));
            }}
            className={`rounded-full border px-3 py-1 text-xs eyebrow ${
              expansion === exp ? 'border-turquoise text-turquoise' : 'border-border text-text-muted'
            }`}
          >
            {exp}
          </button>
        ))}
      </div>

      {isLoading && <p className="text-text-muted">Lade Historie…</p>}

      <div className="flex flex-col gap-3">
        {grouped.map(({ expansion: exp, tiers: expTiers }) => (
          <ExpansionSection
            key={exp}
            expansion={exp}
            tiers={expTiers}
            open={openSet.has(exp)}
            onToggle={() => toggle(exp)}
          />
        ))}
      </div>
    </div>
  );
}

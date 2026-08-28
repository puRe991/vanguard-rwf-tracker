import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useHistory } from '../hooks/useHistory';

export function History() {
  const [expansion, setExpansion] = useState<string | undefined>();
  const { data: tiers, isLoading } = useHistory({ expansion });

  const expansions = [
    'The War Within',
    'Dragonflight',
    'Shadowlands',
    'Cataclysm',
    'Wrath of the Lich King',
    'Classic',
  ];

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
        {expansions.map((exp) => (
          <button
            key={exp}
            onClick={() => setExpansion(exp)}
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
        {tiers?.map((tier) => (
          <div
            key={`${tier.expansion}-${tier.season}-${tier.raidName}`}
            className="flex items-center justify-between rounded-[10px] border border-border bg-card p-4"
          >
            <div>
              <div className="eyebrow text-[11px] text-text-muted">
                {tier.expansion} · Season {tier.season}
              </div>
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
        ))}
      </div>
    </div>
  );
}

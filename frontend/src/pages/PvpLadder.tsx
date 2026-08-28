import { useState } from 'react';
import { usePvpLadder } from '../hooks/usePvpLadder';
import { RatingRail } from '../components/RatingRail';
import { BetaBadge } from '../components/BetaBadge';
import type { PvpBracket } from '../types';

const BRACKETS: { slug: PvpBracket; label: string }[] = [
  { slug: '3v3', label: '3v3' },
  { slug: '2v2', label: '2v2' },
  { slug: 'rbg', label: 'Rated Battleground' },
  { slug: 'solo-shuffle', label: 'Solo Shuffle' },
];

const TIER_TEXT: Record<string, string> = {
  Gladiator: 'text-ember-light',
  Elite: 'text-gold-light',
  Duelist: 'text-gold-light',
  Rival: 'text-turquoise',
  Challenger: 'text-text',
  Combatant: 'text-text-muted',
};

export function PvpLadder() {
  const [bracket, setBracket] = useState<PvpBracket>('3v3');
  const { data: entries, isLoading } = usePvpLadder(bracket);

  return (
    <div>
      <div className="mb-1 flex flex-wrap items-center gap-3">
        <p className="eyebrow text-xs text-turquoise">PvP</p>
        <BetaBadge />
      </div>
      <h1 className="font-headline mb-2 text-4xl">Rating-Leiter</h1>
      <p className="mb-6 max-w-2xl text-sm text-text-muted">
        Beta-Vorschau ohne Blizzard-Battle.net-Anbindung — die Ratings unten sind
        kuratierte Platzhalterdaten (fiktive Team-/Spielernamen), keine echten
        Ladder-Stände. Live-Daten folgen, sobald eine offizielle API-Quelle angebunden ist.
      </p>

      <div className="mb-6 flex flex-wrap gap-2">
        {BRACKETS.map((b) => (
          <button
            key={b.slug}
            onClick={() => setBracket(b.slug)}
            className={`rounded-full border px-3 py-1 text-xs eyebrow ${
              bracket === b.slug ? 'border-turquoise text-turquoise' : 'border-border text-text-muted'
            }`}
          >
            {b.label}
          </button>
        ))}
      </div>

      {isLoading && <p className="text-text-muted">Lade Rating-Leiter…</p>}

      <div className="overflow-hidden rounded-[10px] border border-border bg-card">
        <table className="w-full text-left text-sm">
          <thead>
            <tr className="border-b border-border text-text-muted">
              <th className="eyebrow px-4 py-3 text-[11px] font-normal">#</th>
              <th className="eyebrow px-4 py-3 text-[11px] font-normal">Team</th>
              <th className="eyebrow px-4 py-3 text-[11px] font-normal">Region</th>
              <th className="eyebrow px-4 py-3 text-[11px] font-normal">Rating</th>
              <th className="eyebrow px-4 py-3 text-[11px] font-normal">Tier</th>
              <th className="eyebrow px-4 py-3 text-[11px] font-normal">Fortschritt</th>
            </tr>
          </thead>
          <tbody>
            {entries?.map((entry) => (
              <tr key={entry.id} className="border-b border-border last:border-0 hover:bg-obsidian/40">
                <td className="font-mono-num px-4 py-3 text-text-muted">{entry.rank}</td>
                <td className="px-4 py-3">
                  <div className="text-text">{entry.name}</div>
                  <div className="text-xs text-text-muted">{entry.players.join(' · ')}</div>
                </td>
                <td className="px-4 py-3 text-text-muted">{entry.region}</td>
                <td className="font-mono-num px-4 py-3 text-text">{entry.rating}</td>
                <td className={`px-4 py-3 font-medium ${TIER_TEXT[entry.tier] ?? 'text-text'}`}>
                  {entry.tier}
                </td>
                <td className="px-4 py-3">
                  <RatingRail tier={entry.tier} />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

import { useState } from 'react';

export function SubmitKill() {
  const [submitted, setSubmitted] = useState(false);

  return (
    <div>
      <p className="eyebrow text-xs text-turquoise">Community-Beitrag</p>
      <h1 className="font-headline mb-2 text-4xl">Historischen Kill einreichen</h1>
      <p className="mb-6 max-w-xl text-text-muted">
        Für Classic bis WotLK gibt es keine verlässliche API. Reiche einen Kill mit Beleg
        (Screenshot, Forenpost-Link oder Video-Timestamp) ein — ein Moderator prüft ihn vor
        Veröffentlichung.
      </p>

      {submitted ? (
        <div className="rounded-[10px] border border-turquoise/50 bg-card p-4 text-turquoise">
          Danke! Dein Beitrag wurde zur Moderation eingereicht.
        </div>
      ) : (
        <form
          className="flex max-w-xl flex-col gap-4"
          onSubmit={(e) => {
            e.preventDefault();
            // Auth (JWT) erforderlich — Anbindung an POST /api/kills/submit folgt mit Phase 4.
            setSubmitted(true);
          }}
        >
          <Field label="Gilde" name="guild" required />
          <Field label="Boss" name="boss" required />
          <Field label="Zeitpunkt des Kills" name="timestamp" type="datetime-local" required />
          <Field label="Pull-Anzahl" name="pulls" type="number" min={1} />
          <Field label="Beleg-Link" name="sourceUrl" type="url" required />
          <button
            type="submit"
            className="eyebrow mt-2 rounded-[10px] border border-gold bg-gold/15 px-4 py-2 text-sm text-gold-light hover:bg-gold/25"
          >
            Einreichen
          </button>
        </form>
      )}
    </div>
  );
}

function Field({
  label,
  name,
  type = 'text',
  required,
  min,
}: {
  label: string;
  name: string;
  type?: string;
  required?: boolean;
  min?: number;
}) {
  return (
    <label className="flex flex-col gap-1 text-sm">
      <span className="eyebrow text-[11px] text-text-muted">{label}</span>
      <input
        name={name}
        type={type}
        required={required}
        min={min}
        className="rounded-[10px] border border-border bg-card px-3 py-2 text-text outline-none focus:border-turquoise"
      />
    </label>
  );
}

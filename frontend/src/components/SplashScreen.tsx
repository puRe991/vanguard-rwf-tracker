import { useEffect, useState } from 'react';
import { APP_VERSION } from '../lib/version';

const NODE_COUNT = 6;
const VISIBLE_MS = 1500;
const FADE_MS = 300;

export function SplashScreen({ onDone }: { onDone: () => void }) {
  const [fadingOut, setFadingOut] = useState(false);

  useEffect(() => {
    const fadeTimer = setTimeout(() => setFadingOut(true), VISIBLE_MS);
    const doneTimer = setTimeout(onDone, VISIBLE_MS + FADE_MS);
    return () => {
      clearTimeout(fadeTimer);
      clearTimeout(doneTimer);
    };
  }, [onDone]);

  return (
    <div
      aria-hidden={fadingOut}
      className={`fixed inset-0 z-[100] flex flex-col items-center justify-center gap-10 bg-obsidian transition-opacity duration-300 ease-out ${
        fadingOut ? 'pointer-events-none opacity-0' : 'opacity-100'
      }`}
    >
      <div className="flex flex-col items-center gap-3 text-center">
        <div
          className="font-headline text-6xl text-text"
          style={{ animation: 'splash-logo-in 0.7s ease-out both' }}
        >
          VAN<span className="text-turquoise">GUARD</span>
        </div>
        <p
          className="eyebrow text-xs text-text-muted"
          style={{ animation: 'splash-eyebrow-in 1.1s ease-out both' }}
        >
          Race to World First Tracker
        </p>
      </div>

      <div className="flex gap-2" role="status" aria-label="Lädt…">
        {Array.from({ length: NODE_COUNT }).map((_, i) => (
          <span
            key={i}
            className="h-2 w-2 rounded-full bg-border"
            style={{ animation: 'splash-node 1.4s ease-in-out infinite', animationDelay: `${i * 0.12}s` }}
          />
        ))}
      </div>

      <div className="absolute bottom-8 font-mono-num text-[11px] text-text-muted">v{APP_VERSION}</div>
    </div>
  );
}

import { useEffect, useState } from 'react';
import { getRaceHubConnection } from '../lib/raceHubConnection';
import type { LiveTickerEvent } from '../types';

const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';
const TOAST_LIFETIME_MS = 8000;

export interface KillToast extends LiveTickerEvent {
  toastId: string;
}

/**
 * Transiente Kill-Benachrichtigungen app-weit (nicht nur im Dashboard-Ticker).
 * Zeigt bewusst nur "kill"-Events, keine Pull-Milestones — sonst zu viel Rauschen.
 * Im Mock-Modus bewusst leer: ohne echten Hub gäbe es sonst vorgetäuschte Live-Kills.
 */
export function useKillToasts() {
  const [toasts, setToasts] = useState<KillToast[]>([]);

  useEffect(() => {
    if (USE_MOCKS) return;

    const connection = getRaceHubConnection();
    const timers = new Map<string, ReturnType<typeof setTimeout>>();

    const onTickerEvent = (event: LiveTickerEvent) => {
      if (event.kind !== 'kill') return;

      const toastId = `${event.id}-${Date.now()}`;
      setToasts((prev) => [...prev, { ...event, toastId }]);

      timers.set(
        toastId,
        setTimeout(() => {
          setToasts((prev) => prev.filter((t) => t.toastId !== toastId));
          timers.delete(toastId);
        }, TOAST_LIFETIME_MS),
      );
    };

    connection.on('TickerEvent', onTickerEvent);
    return () => {
      connection.off('TickerEvent', onTickerEvent);
      timers.forEach((timer) => clearTimeout(timer));
    };
  }, []);

  function dismiss(toastId: string) {
    setToasts((prev) => prev.filter((t) => t.toastId !== toastId));
  }

  return { toasts, dismiss };
}

import { useKillToasts } from '../hooks/useKillToasts';

export function KillToastStack() {
  const { toasts, dismiss } = useKillToasts();

  if (toasts.length === 0) return null;

  return (
    <div className="pointer-events-none fixed bottom-4 right-4 z-50 flex w-80 flex-col gap-2">
      {toasts.map((toast) => (
        <div
          key={toast.toastId}
          role="status"
          className="pointer-events-auto animate-[toast-in_0.2s_ease-out] rounded-[10px] border border-gold bg-card p-4 shadow-lg shadow-black/40"
        >
          <div className="flex items-start justify-between gap-3">
            <div>
              <div className="eyebrow text-[11px] text-gold-light">Neuer Weltrekord-Kill</div>
              <p className="mt-1 text-sm text-text">{toast.message}</p>
            </div>
            <button
              onClick={() => dismiss(toast.toastId)}
              className="text-text-muted hover:text-text"
              aria-label="Benachrichtigung schließen"
            >
              ✕
            </button>
          </div>
        </div>
      ))}
    </div>
  );
}

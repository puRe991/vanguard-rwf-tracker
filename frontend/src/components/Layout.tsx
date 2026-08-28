import { NavLink, Outlet } from 'react-router-dom';
import { LiveStatusBadge } from './LiveStatusBadge';
import { KillToastStack } from './KillToastStack';
import { APP_VERSION } from '../lib/version';

const links = [
  { to: '/', label: 'Live-Race', end: true },
  { to: '/history', label: 'Historie' },
  { to: '/submit', label: 'Kill einreichen' },
];

export function Layout() {
  return (
    <div className="flex min-h-screen flex-col bg-obsidian text-text">
      <header className="border-b border-border">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
          <div className="flex items-center gap-4">
            <div className="font-headline text-2xl tracking-wide text-text">
              VAN<span className="text-turquoise">GUARD</span>
            </div>
            <LiveStatusBadge />
          </div>
          <nav className="flex gap-6">
            {links.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                end={link.end}
                className={({ isActive }) =>
                  [
                    'eyebrow text-xs transition-colors',
                    isActive ? 'text-turquoise' : 'text-text-muted hover:text-text',
                  ].join(' ')
                }
              >
                {link.label}
              </NavLink>
            ))}
          </nav>
        </div>
      </header>
      <main className="mx-auto w-full max-w-6xl flex-1 px-6 py-8">
        <Outlet />
      </main>
      <footer className="border-t border-border">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4 text-[11px] text-text-muted">
          <span>Vanguard — Race to World First Tracker</span>
          <span className="font-mono-num">v{APP_VERSION}</span>
        </div>
      </footer>
      <KillToastStack />
    </div>
  );
}

import { NavLink, Outlet } from 'react-router-dom';
import { LiveStatusBadge } from './LiveStatusBadge';
import { KillToastStack } from './KillToastStack';

const links = [
  { to: '/', label: 'Live-Race', end: true },
  { to: '/history', label: 'Historie' },
  { to: '/submit', label: 'Kill einreichen' },
];

export function Layout() {
  return (
    <div className="min-h-screen bg-obsidian text-text">
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
      <main className="mx-auto max-w-6xl px-6 py-8">
        <Outlet />
      </main>
      <KillToastStack />
    </div>
  );
}

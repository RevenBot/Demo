import { type ReactNode } from 'react';
import { Link, useLocation } from 'wouter';
import styles from './PageShell.module.css';

export interface NavItem {
  label: string;
  href: string;
}

export interface PageShellProps {
  children: ReactNode;
  nav?: NavItem[];
}

const DEFAULT_NAV: NavItem[] = [
  { label: 'Home', href: '/' },
  { label: 'Products', href: '/products' },
  { label: 'Restaurants', href: '/restaurants' },
];

function isNavItemActive(location: string, href: string): boolean {
  if (href === '/') return location === '/';
  return location === href || location.startsWith(`${href}/`);
}

export function PageShell({ children, nav = DEFAULT_NAV }: PageShellProps) {
  const [location] = useLocation();

  return (
    <div className={styles.shell}>
      <nav className={styles.nav} aria-label="Primary">
        <span className={styles.brand}>Demo</span>
        <ul className={styles.navList}>
          {nav.map((item) => {
            const active = isNavItemActive(location, item.href);
            return (
              <li key={item.href}>
                <Link
                  to={item.href}
                  className={active ? `${styles.navLink} ${styles.navLinkActive}` : styles.navLink}
                  aria-current={active ? 'page' : undefined}
                >
                  {item.label}
                </Link>
              </li>
            );
          })}
        </ul>
      </nav>
      <main className={styles.content}>
        {children}
      </main>
    </div>
  );
}

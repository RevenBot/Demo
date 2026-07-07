import { type ReactNode } from 'react';
import styles from './StatusBanner.module.css';

export type StatusBannerVariant = 'error' | 'empty' | 'loading';

export interface StatusBannerProps {
  variant: StatusBannerVariant;
  children: ReactNode;
}

export function StatusBanner({ variant, children }: StatusBannerProps) {
  const role = variant === 'error' ? 'alert' : 'status';

  return (
    <div
      className={styles.banner}
      data-variant={variant}
      role={role}
      aria-busy={variant === 'loading' || undefined}
    >
      {variant === 'loading' && <span className={styles.spinner} aria-hidden="true" />}
      <span className={styles.text}>{children}</span>
    </div>
  );
}

import { Card } from 'primereact/card';
import type { PropsWithChildren, ReactNode } from 'react';

interface AppCardProps extends PropsWithChildren {
  action?: ReactNode;
  className?: string;
  title?: ReactNode;
}

export const AppCard = ({ action, children, className, title }: AppCardProps) => {
  let cardClassName = 'rounded-md bg-white p-6 shadow-app-card';

  if (className) cardClassName = `${cardClassName} ${className}`;

  return (
    <Card className={cardClassName}>
      {(title ?? action) && (
        <div className="mb-5 flex items-center justify-between gap-4">
          {title && <h2 className="text-base font-bold text-brand-heading">{title}</h2>}
          {action}
        </div>
      )}
      {children}
    </Card>
  );
};

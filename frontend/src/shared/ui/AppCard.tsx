import { Card } from 'primereact/card';
import type { PropsWithChildren, ReactNode } from 'react';
import { mergeClassNames } from './mergeClassNames';

interface AppCardProps extends PropsWithChildren {
  action?: ReactNode;
  className?: string;
  fillHeight?: boolean;
  title?: ReactNode;
}

const fillHeightClassName =
  'flex h-full min-h-0 flex-col overflow-hidden [&_.p-card-body]:flex [&_.p-card-body]:min-h-0 [&_.p-card-body]:flex-1 [&_.p-card-body]:flex-col [&_.p-card-content]:flex [&_.p-card-content]:min-h-0 [&_.p-card-content]:flex-1 [&_.p-card-content]:flex-col';

export const AppCard = ({ action, children, className, fillHeight = false, title }: AppCardProps) => {
  return (
    <Card className={mergeClassNames('rounded-md bg-white p-6 shadow-app-card', fillHeight && fillHeightClassName, className)}>
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

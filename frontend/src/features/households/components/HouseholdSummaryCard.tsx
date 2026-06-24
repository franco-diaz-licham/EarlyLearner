import type { ReactNode } from 'react';
import { AppCard } from '../../../shared/ui/AppCard';

interface HouseholdSummaryCardProps {
  children: ReactNode;
  emptyMessage?: string;
  isEmpty?: boolean;
  title: string;
}

export const HouseholdSummaryCard = ({ children, emptyMessage, isEmpty = false, title }: HouseholdSummaryCardProps) => {
  return (
    <AppCard title={title}>
      {isEmpty && emptyMessage ? <p className="text-sm text-brand-muted">{emptyMessage}</p> : null}
      {children}
    </AppCard>
  );
};

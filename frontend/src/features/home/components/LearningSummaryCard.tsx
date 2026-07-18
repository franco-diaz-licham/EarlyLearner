import { Link } from 'react-router-dom';
import { UilPlus } from '@iconscout/react-unicons';
import type { HomeTodaySummaryModel } from '../types/home.types';

interface LearningSummaryCardProps {
  isLoading: boolean;
  today?: HomeTodaySummaryModel;
}

export const LearningSummaryCard = ({ isLoading, today }: LearningSummaryCardProps) => {
  return (
    <div className="rounded-md bg-white p-5 shadow-app-card">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h2 className="text-base font-bold text-brand-heading">Today</h2>
          <p className="mt-1 text-sm text-brand-muted">A quick pulse of learning captured today.</p>
        </div>
        <Link className="inline-flex min-h-10 items-center justify-center gap-2 rounded-md bg-brand-primary px-4 text-sm font-semibold text-white hover:bg-brand-primary-hover" to="/learning">
          <UilPlus aria-hidden="true" className="h-4 w-4" />
          Add log
        </Link>
      </div>

      <div className="mt-5 grid gap-3 sm:grid-cols-3">
        <div className="rounded-md bg-brand-primary-soft p-4">
          <p className="text-3xl font-bold text-brand-heading">{isLoading ? '-' : (today?.dailyLogCount ?? 0)}</p>
          <p className="mt-1 text-sm font-semibold text-brand-muted">Daily logs</p>
        </div>
        <div className="rounded-md bg-brand-sky-50 p-4">
          <p className="text-3xl font-bold text-brand-heading">{isLoading ? '-' : (today?.learningMomentCount ?? 0)}</p>
          <p className="mt-1 text-sm font-semibold text-brand-muted">Learning moments</p>
        </div>
        <div className="rounded-md bg-brand-mint-50 p-4">
          <p className="text-3xl font-bold text-brand-heading">{isLoading ? '-' : (today?.childrenObservedCount ?? 0)}</p>
          <p className="mt-1 text-sm font-semibold text-brand-muted">Children observed</p>
        </div>
      </div>
    </div>
  );
};

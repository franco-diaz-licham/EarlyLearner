import { Link } from 'react-router-dom';
import { UilShieldCheck } from '@iconscout/react-unicons';
import type { HomeOutcomeCoverageModel } from '../types/home.types';

interface LearningCoverageCardProps {
  coverage?: HomeOutcomeCoverageModel;
  isLoading: boolean;
}

export const LearningCoverageCard = ({ coverage, isLoading }: LearningCoverageCardProps) => {
  return (
    <div className="rounded-md bg-white p-5 shadow-app-card">
      <div className="flex items-center gap-3">
        <div className="flex h-10 w-10 items-center justify-center rounded-md bg-brand-sky-50 text-brand-sky-500">
          <UilShieldCheck aria-hidden="true" size={20} />
        </div>
        <div>
          <h2 className="text-base font-bold text-brand-heading">Learning Coverage</h2>
          <p className="text-sm text-brand-muted">Outcome use over the last seven days.</p>
        </div>
      </div>

      <div className="mt-5 grid grid-cols-3 gap-2">
        <div className="rounded-md bg-brand-surface-soft p-3 text-center">
          <p className="text-2xl font-bold text-brand-heading">{isLoading ? '-' : (coverage?.activeOutcomeCount ?? 0)}</p>
          <p className="mt-1 text-xs font-semibold text-brand-muted">Active</p>
        </div>
        <div className="rounded-md bg-brand-mint-50 p-3 text-center">
          <p className="text-2xl font-bold text-brand-heading">{isLoading ? '-' : (coverage?.touchedThisWeekCount ?? 0)}</p>
          <p className="mt-1 text-xs font-semibold text-brand-muted">Used</p>
        </div>
        <div className="rounded-md bg-brand-yellow-50 p-3 text-center">
          <p className="text-2xl font-bold text-brand-heading">{isLoading ? '-' : (coverage?.untouchedActiveOutcomeCount ?? 0)}</p>
          <p className="mt-1 text-xs font-semibold text-brand-muted">Not used</p>
        </div>
      </div>

      <Link className="mt-4 inline-flex text-sm font-semibold text-brand-primary hover:text-brand-primary-hover" to="/learning">
        Manage outcomes
      </Link>
    </div>
  );
};


import { UilCalendarAlt } from '@iconscout/react-unicons';
import type { HomeMetricModel } from '../types/home.types';

interface HouseholdPulseCardProps {
  metrics: HomeMetricModel[];
}

export const HouseholdPulseCard = ({ metrics }: HouseholdPulseCardProps) => {
  return (
    <div className="rounded-md bg-white p-5 shadow-app-card">
      <div className="flex items-center gap-3">
        <div className="flex h-10 w-10 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
          <UilCalendarAlt aria-hidden="true" size={20} />
        </div>
        <div>
          <h2 className="text-base font-bold text-brand-heading">Household Pulse</h2>
          <p className="text-sm text-brand-muted">Small operational numbers for today.</p>
        </div>
      </div>

      <div className="mt-5 space-y-3">
        {metrics.map((metric) => (
          <div className="flex items-start justify-between gap-4 rounded-md border border-brand-border p-3" key={metric.label}>
            <div>
              <p className="text-sm font-bold text-brand-heading">{metric.label}</p>
              <p className="mt-1 text-xs leading-5 text-brand-muted">{metric.detail}</p>
            </div>
            <p className="text-2xl font-bold text-brand-heading">{metric.value}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

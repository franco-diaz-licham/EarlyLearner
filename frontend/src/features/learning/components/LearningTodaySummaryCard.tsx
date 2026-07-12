import { AppCard } from '../../../shared/ui/AppCard';

interface LearningTodaySummaryCardProps {
  activityCount: number;
  readingCount: number;
  routineCount: number;
}

export const LearningTodaySummaryCard = ({ activityCount, readingCount, routineCount }: LearningTodaySummaryCardProps) => {
  return (
    <AppCard title="Today at a Glance">
      <div className="grid grid-cols-3 gap-3 text-center">
        <div className="rounded-md bg-brand-surface-muted p-3">
          <p className="text-2xl font-bold text-brand-heading">{activityCount}</p>
          <p className="text-xs font-semibold text-brand-muted">Activities</p>
        </div>
        <div className="rounded-md bg-brand-surface-muted p-3">
          <p className="text-2xl font-bold text-brand-heading">{readingCount}</p>
          <p className="text-xs font-semibold text-brand-muted">Reading</p>
        </div>
        <div className="rounded-md bg-brand-surface-muted p-3">
          <p className="text-2xl font-bold text-brand-heading">{routineCount}</p>
          <p className="text-xs font-semibold text-brand-muted">Routine</p>
        </div>
      </div>
    </AppCard>
  );
};

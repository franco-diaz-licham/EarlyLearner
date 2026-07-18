import { UilBookOpen, UilCalendarAlt, UilTrashAlt } from '@iconscout/react-unicons';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import type { LearningMomentKind } from '../types/dailyLog.types';

export interface LearningMomentListItem {
  learningMomentId: string;
  dailyLogId: string;
  childId: string;
  logDate: string;
  kind: LearningMomentKind;
  title: string;
  notes: string;
  learningOutcomeIds: string[];
}

interface LearningMomentListProps {
  hasMore: boolean;
  isFetchingMore: boolean;
  isDeleting: boolean;
  moments: LearningMomentListItem[];
  searchTerm: string;
  onDeleteMoment: (dailyLogId: string, learningMomentId: string) => void;
  onLoadMore: () => void;
  onSearchTermChange: (searchTerm: string) => void;
}

const learningKindLabels: Record<LearningMomentKind, string> = {
  activity: 'Activity',
  observation: 'Observation',
  reading: 'Reading',
  routine: 'Routine'
};

const formatDisplayDate = (value: string) =>
  new Intl.DateTimeFormat(undefined, {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(`${value}T00:00:00`));

export const LearningMomentList = ({ hasMore, isDeleting, isFetchingMore, moments, searchTerm, onDeleteMoment, onLoadMore, onSearchTermChange }: LearningMomentListProps) => {
  const handleScroll = (event: React.UIEvent<HTMLDivElement>) => {
    if (!hasMore || isFetchingMore) return;
    const { clientHeight, scrollHeight, scrollTop } = event.currentTarget;
    if (scrollHeight - scrollTop - clientHeight <= 160) onLoadMore();
  };

  return (
    <AppCard title="Recent Learning">
      <div className="mb-4">
        <AppInputText
          aria-label="Search recent learning"
          placeholder="Search recent learning"
          value={searchTerm}
          onChange={(event) => {
            onSearchTermChange(event.target.value);
          }}
        />
      </div>
      {moments.length === 0 ? (
        <p className="text-sm text-brand-muted">No learning moments recorded yet.</p>
      ) : (
        <div className="space-y-3 max-h-[calc(100vh-28rem)] overflow-y-auto pr-1" onScroll={handleScroll}>
          {moments.map((moment) => (
            <article className="rounded-md border border-brand-border p-4" key={moment.learningMomentId}>
              <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
                <div className="flex min-w-0 items-start gap-4">
                  <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
                    {moment.kind === 'reading' ? <UilBookOpen aria-hidden="true" className="h-5 w-5" /> : <UilCalendarAlt aria-hidden="true" className="h-5 w-5" />}
                  </div>
                  <div className="min-w-0">
                    <h2 className="font-bold text-brand-heading">{moment.title}</h2>
                    <p className="mt-2 text-sm leading-6 text-brand-muted">{moment.notes}</p>
                    <div className="mt-3 flex flex-wrap items-center gap-2">
                      <AppStatusBadge>{learningKindLabels[moment.kind]}</AppStatusBadge>
                      <span className="text-xs font-semibold text-brand-muted">{formatDisplayDate(moment.logDate)}</span>
                    </div>
                  </div>
                </div>
                <AppIconButton
                  aria-label={`Delete ${moment.title}`}
                  disabled={isDeleting}
                  type="button"
                  onClick={() => {
                    onDeleteMoment(moment.dailyLogId, moment.learningMomentId);
                  }}
                >
                  <UilTrashAlt aria-hidden="true" className="h-5 w-5" />
                </AppIconButton>
              </div>
            </article>
          ))}
          {isFetchingMore ? <p className="py-2 text-center text-sm font-semibold text-brand-muted">Loading more...</p> : null}
        </div>
      )}
    </AppCard>
  );
};

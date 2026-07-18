import { AppCard } from '../../../shared/ui/AppCard';
import { AppInputText } from '../../../shared/ui/AppInputText';
import type { LearningMomentKind } from '../types/dailyLog.types';
import { LearningMomentListItemCard } from './LearningMomentListItemCard';

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
        <div className="space-y-3 max-h-[calc(100vh-30rem)] overflow-y-auto pr-1" onScroll={handleScroll}>
          {moments.map((moment) => (
            <LearningMomentListItemCard isDeleting={isDeleting} key={moment.learningMomentId} moment={moment} onDeleteMoment={onDeleteMoment} />
          ))}
          {isFetchingMore ? <p className="py-2 text-center text-sm font-semibold text-brand-muted">Loading more...</p> : null}
        </div>
      )}
    </AppCard>
  );
};

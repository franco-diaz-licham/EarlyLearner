import { UilBookOpen, UilCalendarAlt, UilEditAlt, UilTrashAlt, UilUsersAlt } from '@iconscout/react-unicons';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { formatDisplayDate } from '../../../shared/utils/dateFormat';
import type { LearningMomentKind } from '../types/dailyLog.types';
import type { LearningMomentListItem } from './LearningMomentList';

interface LearningMomentListItemCardProps {
  isDeleting: boolean;
  moment: LearningMomentListItem;
  onDeleteMoment: (dailyLogId: string, learningMomentId: string) => void;
  onEditMoment: (moment: LearningMomentListItem) => void;
}

const learningKindLabels: Record<LearningMomentKind, string> = {
  activity: 'Activity',
  observation: 'Observation',
  reading: 'Reading',
  routine: 'Routine'
};


export const LearningMomentListItemCard = ({ isDeleting, moment, onDeleteMoment, onEditMoment }: LearningMomentListItemCardProps) => {
  return (
    <article className="rounded-md border border-brand-border p-4">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div className="flex min-w-0 items-start gap-4">
          <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
            {moment.kind === 'reading' ? <UilBookOpen aria-hidden="true" className="h-5 w-5" /> : <UilCalendarAlt aria-hidden="true" className="h-5 w-5" />}
          </div>
          <div className="min-w-0">
            <h2 className="font-bold text-brand-heading">{moment.title}</h2>
            <p className="mt-2 text-sm leading-6 text-brand-muted">{moment.notes}</p>
            <div className="mt-3 flex flex-wrap items-center gap-2">
              <span className="inline-flex items-center gap-1 rounded-md bg-brand-surface-muted px-2 py-1 text-xs font-semibold text-brand-text">
                {moment.kind === 'reading' ? <UilBookOpen aria-hidden="true" className="h-3.5 w-3.5" /> : <UilCalendarAlt aria-hidden="true" className="h-3.5 w-3.5" />}
                {learningKindLabels[moment.kind]}
              </span>
              <span className="inline-flex items-center gap-1 rounded-md bg-brand-sky-50 px-2 py-1 text-xs font-semibold text-brand-muted">
                <UilUsersAlt aria-hidden="true" className="h-3.5 w-3.5" />
                {moment.childName}
              </span>
              <span className="text-xs font-semibold text-brand-muted">{formatDisplayDate(moment.logDate)}</span>
            </div>
          </div>
        </div>
        <div className="flex items-center">
          <AppIconButton
            aria-label={`Edit ${moment.title}`}
            className="flex"
            onClick={() => {
              onEditMoment(moment);
            }}
          >
            <UilEditAlt aria-hidden="true" className="h-5 w-5" />
          </AppIconButton>
          <AppIconButton
            aria-label={`Delete ${moment.title}`}
            className="flex text-brand-error hover:bg-brand-primary-soft"
            disabled={isDeleting}
            onClick={() => {
              onDeleteMoment(moment.dailyLogId, moment.learningMomentId);
            }}
          >
            <UilTrashAlt aria-hidden="true" className="h-5 w-5" />
          </AppIconButton>
        </div>
      </div>
    </article>
  );
};

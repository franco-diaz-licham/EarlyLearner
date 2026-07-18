import { UilEditAlt, UilTrashAlt } from '@iconscout/react-unicons';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppSelect } from '../../../shared/ui/AppSelect';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';

interface LearningOutcomeListProps {
  deletingId?: string | null;
  outcomes: LearningOutcomeModel[];
  updatingStatusId?: string | null;
  onDeleteOutcome: (outcome: LearningOutcomeModel) => void;
  onEditOutcome: (outcome: LearningOutcomeModel) => void;
  onStatusChange: (outcome: LearningOutcomeModel, status: LearningOutcomeModel['status']) => void;
}

const getStatusLabel = (status: LearningOutcomeModel['status']) => {
  if (status === LearningOutcomeStatus.Active) return 'Active';
  if (status === LearningOutcomeStatus.Inactive) return 'Inactive';
  return 'Archived';
};

const getStatusTone = (status: LearningOutcomeModel['status']) => (status === LearningOutcomeStatus.Active ? 'success' : status === LearningOutcomeStatus.Inactive ? 'warning' : 'neutral');

const learningOutcomeStatusOptions = [
  { label: 'Active', value: LearningOutcomeStatus.Active },
  { label: 'Inactive', value: LearningOutcomeStatus.Inactive },
  { label: 'Archived', value: LearningOutcomeStatus.Archived }
];

export const LearningOutcomeList = ({ deletingId, outcomes, updatingStatusId, onDeleteOutcome, onEditOutcome, onStatusChange }: LearningOutcomeListProps) => {
  return (
    <AppCard className="max-h-[calc(100dvh-31rem)]" fillHeight>
      <div>
        <h2 className="text-base font-bold text-brand-heading">Learning Outcomes</h2>
        <p className="mt-1 text-sm text-brand-muted">{outcomes.length} configured</p>
      </div>

      <div className="mt-5 grid min-h-0 flex-1 gap-3 overflow-y-auto pr-1 md:grid-cols-2 xl:grid-cols-3">
        {outcomes.length === 0 ? (
          <p className="text-sm text-brand-muted md:col-span-2 xl:col-span-3">No learning outcomes configured yet.</p>
        ) : (
          outcomes.map((outcome) => (
            <div className="flex min-h-0 flex-col rounded-md border border-brand-border bg-white p-3" key={outcome.learningOutcomeId}>
              <div className="min-w-0">
                <div className="flex flex-wrap items-start justify-between gap-2">
                  <h3 className="min-w-0 text-base font-semibold leading-6 text-brand-heading">{outcome.name}</h3>
                  <AppStatusBadge tone={getStatusTone(outcome.status)}>{getStatusLabel(outcome.status)}</AppStatusBadge>
                </div>
                <p className="mt-1 text-xs font-semibold text-brand-muted">{outcome.category}</p>
                <p className="mt-2 text-sm leading-6 text-brand-muted">{outcome.description}</p>
              </div>
              <div className="mt-auto flex flex-wrap items-center justify-between gap-2 border-t border-brand-border/70 pt-3">
                <AppSelect
                  aria-label={`Status for ${outcome.name}`}
                  className="min-h-9 w-32 text-sm"
                  disabled={updatingStatusId === outcome.learningOutcomeId}
                  options={learningOutcomeStatusOptions}
                  value={outcome.status}
                  onChange={(status) => {
                    if (status === outcome.status) return;
                    onStatusChange(outcome, status);
                  }}
                />
                <div className="flex">
                  <AppIconButton
                    aria-label={`Edit ${outcome.name}`}
                    className="flex"
                    onClick={() => {
                      onEditOutcome(outcome);
                    }}
                  >
                    <UilEditAlt aria-hidden="true" className="h-5 w-5" />
                  </AppIconButton>
                  <AppIconButton
                    aria-label={`Delete ${outcome.name}`}
                    className="flex text-brand-error hover:bg-brand-primary-soft"
                    disabled={deletingId === outcome.learningOutcomeId}
                    onClick={() => {
                      onDeleteOutcome(outcome);
                    }}
                  >
                    <UilTrashAlt aria-hidden="true" className="h-5 w-5" />
                  </AppIconButton>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </AppCard>
  );
};

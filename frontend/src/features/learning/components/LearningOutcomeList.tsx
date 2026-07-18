import { UilEditAlt, UilPlus, UilTrashAlt } from '@iconscout/react-unicons';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppSelect } from '../../../shared/ui/AppSelect';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';

interface LearningOutcomeListProps {
  deletingId?: string | null;
  outcomes: LearningOutcomeModel[];
  updatingStatusId?: string | null;
  onAddOutcome: () => void;
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

export const LearningOutcomeList = ({ deletingId, outcomes, updatingStatusId, onAddOutcome, onDeleteOutcome, onEditOutcome, onStatusChange }: LearningOutcomeListProps) => {
  return (
    <AppCard className="max-h-[calc(100dvh-31rem)]" fillHeight>
      <div className="flex items-start justify-between">
        <div>
          <h2 className="text-base font-bold text-brand-heading">Learning Outcomes</h2>
          <p className="mt-1 text-sm text-brand-muted">{outcomes.length} configured</p>
        </div>
        <AppButton icon={<UilPlus aria-hidden="true" className="h-4 w-4" />} label="Add" type="button" onClick={onAddOutcome} />
      </div>

      <div className="mt-5 min-h-0 flex-1 space-y-3 overflow-y-auto pr-1">
        {outcomes.length === 0 ? (
          <p className="text-sm text-brand-muted">No learning outcomes configured yet.</p>
        ) : (
          outcomes.map((outcome) => (
            <div className="rounded-md border border-brand-border bg-white p-3" key={outcome.learningOutcomeId}>
              <div className="min-w-0">
                <div className="flex flex-wrap items-start justify-between">
                  <h3 className="min-w-0 text-base font-semibold leading-6 text-brand-heading">{outcome.name}</h3>
                  <AppStatusBadge tone={getStatusTone(outcome.status)}>{getStatusLabel(outcome.status)}</AppStatusBadge>
                </div>
                <p className="mt-1 text-xs font-semibold text-brand-muted">{outcome.category}</p>
                <p className="mt-2 text-sm leading-6 text-brand-muted">{outcome.description}</p>
              </div>
              <div className="mt-4 flex flex-wrap items-center justify-between gap-2 border-t border-brand-border/70 pt-3">
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
                <div>
                  {' '}
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

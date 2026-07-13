import { UilEditAlt, UilPlus, UilTrashAlt } from '@iconscout/react-unicons';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';

interface LearningOutcomeListProps {
  archivingId?: string | null;
  outcomes: LearningOutcomeModel[];
  onAddOutcome: () => void;
  onArchiveOutcome: (outcome: LearningOutcomeModel) => void;
  onEditOutcome: (outcome: LearningOutcomeModel) => void;
}

const getStatusLabel = (status: LearningOutcomeModel['status']) => {
  if (status === LearningOutcomeStatus.Active) return 'Active';
  if (status === LearningOutcomeStatus.Inactive) return 'Inactive';
  return 'Archived';
};

const getStatusTone = (status: LearningOutcomeModel['status']) => (status === LearningOutcomeStatus.Active ? 'success' : status === LearningOutcomeStatus.Inactive ? 'warning' : 'neutral');

export const LearningOutcomeList = ({ archivingId, outcomes, onAddOutcome, onArchiveOutcome, onEditOutcome }: LearningOutcomeListProps) => {
  return (
    <AppCard>
      <div className="flex items-start justify-between gap-3">
        <div>
          <h2 className="text-base font-bold text-brand-heading">Learning Outcomes</h2>
          <p className="mt-1 text-sm text-brand-muted">{outcomes.length} configured</p>
        </div>
        <AppButton icon={<UilPlus aria-hidden="true" className="h-4 w-4" />} label="Add" type="button" onClick={onAddOutcome} />
      </div>

      <div className="mt-5 max-h-[calc(100vh-36rem)] space-y-3 overflow-y-auto pr-1">
        {outcomes.length === 0 ? (
          <p className="text-sm text-brand-muted">No learning outcomes configured yet.</p>
        ) : (
          outcomes.map((outcome) => (
            <article className="rounded-md border border-brand-border bg-white p-3" key={outcome.learningOutcomeId}>
              <div className="flex items-start justify-between gap-3">
                <div className="min-w-0">
                  <div className="flex flex-wrap items-center gap-2">
                    <h3 className="font-semibold text-brand-heading">{outcome.name}</h3>
                    <AppStatusBadge tone={getStatusTone(outcome.status)}>{getStatusLabel(outcome.status)}</AppStatusBadge>
                  </div>
                  <p className="mt-1 text-xs font-semibold text-brand-muted">{outcome.category}</p>
                  <p className="mt-2 text-sm leading-6 text-brand-muted">{outcome.description}</p>
                </div>
                <div className="flex shrink-0 items-center gap-1">
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
                    aria-label={`Archive ${outcome.name}`}
                    className="flex"
                    disabled={outcome.status === LearningOutcomeStatus.Archived || archivingId === outcome.learningOutcomeId}
                    onClick={() => {
                      onArchiveOutcome(outcome);
                    }}
                  >
                    <UilTrashAlt aria-hidden="true" className="h-5 w-5" />
                  </AppIconButton>
                </div>
              </div>
            </article>
          ))
        )}
      </div>
    </AppCard>
  );
};

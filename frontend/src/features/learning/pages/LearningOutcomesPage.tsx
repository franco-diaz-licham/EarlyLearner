import { useState } from 'react';
import { UilPlus, UilShieldCheck } from '@iconscout/react-unicons';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { LearningOutcomeForm } from '../components/LearningOutcomeForm';
import { LearningOutcomeList } from '../components/LearningOutcomeList';
import type { LearningOutcomeFormModel } from '../hooks/useLearningOutcomeForm';
import { useCreateLearningOutcomeMutation, useDeleteLearningOutcomeMutation, useLearningOutcomesQuery, useUpdateLearningOutcomeMutation, useUpdateLearningOutcomeStatusMutation } from '../queries/learningOutcome.queries';
import type { LearningOutcomeModel } from '../types/learningOutcome.types';

const emptyLearningOutcomes: LearningOutcomeModel[] = [];

export const LearningOutcomesPage = () => {
  const [manageOutcome, setManageOutcome] = useState(false);
  const [editingOutcome, setEditingOutcome] = useState<LearningOutcomeModel | null>(null);

  const learningOutcomesQuery = useLearningOutcomesQuery();
  const createLearningOutcomeMutation = useCreateLearningOutcomeMutation();
  const updateLearningOutcomeMutation = useUpdateLearningOutcomeMutation();
  const updateLearningOutcomeStatusMutation = useUpdateLearningOutcomeStatusMutation();
  const deleteLearningOutcomeMutation = useDeleteLearningOutcomeMutation();

  const learningOutcomes = learningOutcomesQuery.data ?? emptyLearningOutcomes;

  const handleOutcomeSave = async (form: LearningOutcomeFormModel) => {
    if (editingOutcome) {
      await updateLearningOutcomeMutation.mutateAsync({
        learningOutcomeId: editingOutcome.learningOutcomeId,
        form
      });
    } else {
      await createLearningOutcomeMutation.mutateAsync(form);
    }

    setEditingOutcome(null);
    setManageOutcome(false);
  };

  const handleOutcomeFormHide = () => {
    setEditingOutcome(null);
    setManageOutcome(false);
  };

  const outcomeSaving = createLearningOutcomeMutation.isPending || updateLearningOutcomeMutation.isPending;
  const deletingId = deleteLearningOutcomeMutation.isPending ? deleteLearningOutcomeMutation.variables : null;
  const updatingStatusId = updateLearningOutcomeStatusMutation.isPending ? updateLearningOutcomeStatusMutation.variables.learningOutcomeId : null;

  return (
    <section aria-labelledby="learning-outcomes-title" className="space-y-5">
      <AppCard>
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div className="flex items-start gap-4">
            <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-md bg-brand-sky-50 text-brand-sky-500">
              <UilShieldCheck aria-hidden="true" className="h-6 w-6" />
            </div>
            <div>
              <p className="text-sm font-semibold text-brand-muted">Learning</p>
              <h1 id="learning-outcomes-title" className="mt-1 text-3xl font-bold text-brand-heading">
                Manage outcomes
              </h1>
              <p className="mt-2 max-w-2xl text-brand-muted">Create and maintain the outcomes used when recording learning logs.</p>
            </div>
          </div>
          <AppButton
            icon={<UilPlus aria-hidden="true" className="h-4 w-4" />}
            label="Add"
            type="button"
            onClick={() => {
              setEditingOutcome(null);
              setManageOutcome(true);
            }}
          />
        </div>
      </AppCard>

      <LearningOutcomeList
        deletingId={deletingId}
        outcomes={learningOutcomes}
        updatingStatusId={updatingStatusId}
        onDeleteOutcome={(outcome) => {
          deleteLearningOutcomeMutation.mutate(outcome.learningOutcomeId);
        }}
        onEditOutcome={(outcome) => {
          setEditingOutcome(outcome);
          setManageOutcome(true);
        }}
        onStatusChange={(outcome, status) => {
          updateLearningOutcomeStatusMutation.mutate({
            learningOutcomeId: outcome.learningOutcomeId,
            status
          });
        }}
      />

      <LearningOutcomeForm
        key={editingOutcome?.learningOutcomeId ?? (manageOutcome ? 'add-outcome-open' : 'add-outcome-closed')}
        outcome={editingOutcome}
        saving={outcomeSaving}
        visible={manageOutcome}
        onHide={handleOutcomeFormHide}
        onSave={(form) => {
          void handleOutcomeSave(form);
        }}
      />
    </section>
  );
};

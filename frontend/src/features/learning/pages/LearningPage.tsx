import { useMemo, useState } from 'react';
import { useHouseholdQuery } from '../../households/queries/household.queries';
import { LearningHeader } from '../components/LearningHeader';
import { LearningLogForm } from '../components/LearningLogForm';
import { LearningMomentList } from '../components/LearningMomentList';
import { LearningOutcomeForm } from '../components/LearningOutcomeForm';
import { LearningOutcomeList } from '../components/LearningOutcomeList';
import { LearningTodaySummaryCard } from '../components/LearningTodaySummaryCard';
import type { LearningOutcomeFormModel } from '../hooks/useLearningOutcomeForm';
import { useCreateDailyLogMutation, useDailyLogsQuery, useDeleteLearningMomentMutation } from '../queries/dailyLog.queries';
import { useCreateLearningOutcomeMutation, useDeleteLearningOutcomeMutation, useLearningOutcomesQuery, useUpdateLearningOutcomeMutation, useUpdateLearningOutcomeStatusMutation } from '../queries/learningOutcome.queries';
import type { LearningLogFormModel } from '../types/dailyLog.types';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';

const formatDateInputValue = (date: Date) => date.toISOString().slice(0, 10);
const EMPTY_LEARNING_OUTCOMES: LearningOutcomeModel[] = [];

export const LearningPage = () => {
  const [addLog, setAddLog] = useState(false);
  const [manageOutcome, setManageOutcome] = useState(false);
  const [editingOutcome, setEditingOutcome] = useState<LearningOutcomeModel | null>(null);

  const householdQuery = useHouseholdQuery();
  const learningOutcomesQuery = useLearningOutcomesQuery();
  const dailyLogsQuery = useDailyLogsQuery();
  const createDailyLogMutation = useCreateDailyLogMutation();
  const deleteLearningMomentMutation = useDeleteLearningMomentMutation();
  const createLearningOutcomeMutation = useCreateLearningOutcomeMutation();
  const updateLearningOutcomeMutation = useUpdateLearningOutcomeMutation();
  const updateLearningOutcomeStatusMutation = useUpdateLearningOutcomeStatusMutation();
  const deleteLearningOutcomeMutation = useDeleteLearningOutcomeMutation();

  const children = householdQuery.data?.children ?? [];
  const learningOutcomes = learningOutcomesQuery.data ?? EMPTY_LEARNING_OUTCOMES;
  const activeLearningOutcomes = useMemo(() => learningOutcomes.filter((outcome) => outcome.status === LearningOutcomeStatus.Active), [learningOutcomes]);
  const dailyLogs = dailyLogsQuery.data ?? [];
  const today = formatDateInputValue(new Date());

  const latestMoments = dailyLogs.flatMap((log) =>
    log.learningMoments.map((moment) => ({
      ...moment,
      dailyLogId: log.dailyLogId,
      childId: log.childId,
      logDate: log.logDate
    }))
  );

  const todayLogs = dailyLogs.filter((log) => log.logDate === today);
  const todayActivityCount = todayLogs.flatMap((log) => log.learningMoments).filter((moment) => moment.kind === 'activity' || moment.kind === 'observation').length;
  const todayReadingCount = todayLogs.flatMap((log) => log.learningMoments).filter((moment) => moment.kind === 'reading').length;
  const todayRoutineCount = todayLogs.flatMap((log) => log.learningMoments).filter((moment) => moment.kind === 'routine').length;

  const handleSave = async (form: LearningLogFormModel) => {
    await createDailyLogMutation.mutateAsync(form);
    setAddLog(false);
  };

  const handleOutcomeSave = async (form: LearningOutcomeFormModel) => {
    if (editingOutcome) {
      await updateLearningOutcomeMutation.mutateAsync({
        learningOutcomeId: editingOutcome.learningOutcomeId,
        request: {
          name: form.name,
          description: form.description,
          category: form.category,
          sortOrder: form.sortOrder
        }
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

  return (
    <section aria-labelledby="learning-title" className="space-y-5">
      <LearningHeader
        onAddLog={() => {
          setAddLog(true);
        }}
      />

      <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_460px]">
        <LearningMomentList
          isDeleting={deleteLearningMomentMutation.isPending}
          moments={latestMoments}
          onDeleteMoment={(dailyLogId, learningMomentId) => {
            deleteLearningMomentMutation.mutate({ dailyLogId, learningMomentId });
          }}
        />
        <aside className="space-y-5">
          <LearningTodaySummaryCard activityCount={todayActivityCount} readingCount={todayReadingCount} routineCount={todayRoutineCount} />
          <LearningOutcomeList
            deletingId={deleteLearningOutcomeMutation.variables ?? null}
            outcomes={learningOutcomes}
            updatingStatusId={updateLearningOutcomeStatusMutation.variables?.learningOutcomeId ?? null}
            onAddOutcome={() => {
              setEditingOutcome(null);
              setManageOutcome(true);
            }}
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
                request: { status }
              });
            }}
          />
        </aside>
      </div>
      <LearningLogForm
        key={addLog ? 'add-log-open' : 'add-log-closed'}
        children={children}
        learningOutcomes={activeLearningOutcomes}
        saving={createDailyLogMutation.isPending}
        visible={addLog}
        onHide={() => {
          setAddLog(false);
        }}
        onSave={(form) => {
          void handleSave(form);
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

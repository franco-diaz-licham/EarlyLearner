import { useCallback, useMemo, useState } from 'react';
import { useHouseholdQuery } from '../../households/queries/household.queries';
import type { ChildModel } from '../../households/types/household.types';
import { LearningHeader } from '../components/LearningHeader';
import { LearningLogForm } from '../components/LearningLogForm';
import { LearningMomentList, type LearningMomentListItem } from '../components/LearningMomentList';
import { LearningOutcomeForm } from '../components/LearningOutcomeForm';
import { LearningOutcomeList } from '../components/LearningOutcomeList';
import { LearningTodaySummaryCard } from '../components/LearningTodaySummaryCard';
import type { LearningOutcomeFormModel } from '../hooks/useLearningOutcomeForm';
import { useCreateDailyLogMutation, useDailyLogsQuery, useDeleteLearningMomentMutation, useLearningMomentFeedQuery, useUpdateLearningMomentMutation } from '../queries/dailyLog.queries';
import { useCreateLearningOutcomeMutation, useDeleteLearningOutcomeMutation, useLearningOutcomesQuery, useUpdateLearningOutcomeMutation, useUpdateLearningOutcomeStatusMutation } from '../queries/learningOutcome.queries';
import type { LearningLogFormModel } from '../types/dailyLog.types';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';

const formatDateInputValue = (date: Date) => date.toISOString().slice(0, 10);
const emptyChildren: ChildModel[] = [];
const emptyLearningOutcomes: LearningOutcomeModel[] = [];

export const LearningPage = () => {
  const [addLog, setAddLog] = useState(false);
  const [editingMoment, setEditingMoment] = useState<LearningMomentListItem | null>(null);
  const [manageOutcome, setManageOutcome] = useState(false);
  const [editingOutcome, setEditingOutcome] = useState<LearningOutcomeModel | null>(null);
  const [learningMomentSearchTerm, setLearningMomentSearchTerm] = useState('');

  const householdQuery = useHouseholdQuery();
  const learningOutcomesQuery = useLearningOutcomesQuery();
  const dailyLogsQuery = useDailyLogsQuery();
  const learningMomentFeedQuery = useLearningMomentFeedQuery({
    pageSize: 10,
    searchTerm: learningMomentSearchTerm.trim() || null
  });
  const createDailyLogMutation = useCreateDailyLogMutation();
  const updateLearningMomentMutation = useUpdateLearningMomentMutation();
  const deleteLearningMomentMutation = useDeleteLearningMomentMutation();
  const createLearningOutcomeMutation = useCreateLearningOutcomeMutation();
  const updateLearningOutcomeMutation = useUpdateLearningOutcomeMutation();
  const updateLearningOutcomeStatusMutation = useUpdateLearningOutcomeStatusMutation();
  const deleteLearningOutcomeMutation = useDeleteLearningOutcomeMutation();

  const children = useMemo(() => householdQuery.data?.children ?? emptyChildren, [householdQuery.data?.children]);
  const learningOutcomes = learningOutcomesQuery.data ?? emptyLearningOutcomes;
  const activeLearningOutcomes = useMemo(() => learningOutcomes.filter((outcome) => outcome.status === LearningOutcomeStatus.Active), [learningOutcomes]);
  const dailyLogs = dailyLogsQuery.data ?? [];
  const today = formatDateInputValue(new Date());

  const childNamesById = useMemo(() => new Map(children.map((child) => [child.id, `${child.firstName} ${child.lastName}`])), [children]);
  const latestMoments = useMemo(
    () =>
      learningMomentFeedQuery.data?.pages.flatMap((page) =>
        page.items.map((moment) => ({
          ...moment,
          childName: childNamesById.get(moment.childId) ?? 'Unknown child'
        }))
      ) ?? [],
    [childNamesById, learningMomentFeedQuery.data?.pages]
  );

  const todayLogs = dailyLogs.filter((log) => log.logDate === today);
  const todayActivityCount = todayLogs.flatMap((log) => log.learningMoments).filter((moment) => moment.kind === 'activity' || moment.kind === 'observation').length;
  const todayReadingCount = todayLogs.flatMap((log) => log.learningMoments).filter((moment) => moment.kind === 'reading').length;
  const todayRoutineCount = todayLogs.flatMap((log) => log.learningMoments).filter((moment) => moment.kind === 'routine').length;

  const handleSave = async (form: LearningLogFormModel) => {
    if (editingMoment) {
      await updateLearningMomentMutation.mutateAsync({
        dailyLogId: editingMoment.dailyLogId,
        learningMomentId: editingMoment.learningMomentId,
        form
      });
      setEditingMoment(null);
    } else {
      await createDailyLogMutation.mutateAsync(form);
      setAddLog(false);
    }
  };

  const handleLogFormHide = () => {
    setAddLog(false);
    setEditingMoment(null);
  };

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

  const handleLoadMoreLearningMoments = useCallback(() => {
    if (learningMomentFeedQuery.hasNextPage && !learningMomentFeedQuery.isFetchingNextPage) {
      void learningMomentFeedQuery.fetchNextPage();
    }
  }, [learningMomentFeedQuery]);

  const logSaving = createDailyLogMutation.isPending || updateLearningMomentMutation.isPending;
  const outcomeSaving = createLearningOutcomeMutation.isPending || updateLearningOutcomeMutation.isPending;

  return (
    <section aria-labelledby="learning-title" className="space-y-5">
      <LearningHeader
        onAddLog={() => {
          setEditingMoment(null);
          setAddLog(true);
        }}
      />

      <div className="flex flex-wrap gap-5">
        <div className="flex-1 basis-150">
          <LearningMomentList
            hasMore={learningMomentFeedQuery.hasNextPage}
            isDeleting={deleteLearningMomentMutation.isPending}
            isFetchingMore={learningMomentFeedQuery.isFetchingNextPage}
            moments={latestMoments}
            searchTerm={learningMomentSearchTerm}
            onDeleteMoment={(dailyLogId, learningMomentId) => {
              deleteLearningMomentMutation.mutate({ dailyLogId, learningMomentId });
            }}
            onEditMoment={(moment) => {
              setAddLog(false);
              setEditingMoment(moment);
            }}
            onLoadMore={handleLoadMoreLearningMoments}
            onSearchTermChange={setLearningMomentSearchTerm}
          />
        </div>
        <aside className="flex flex-1 basis-115 flex-col gap-3 xl:max-w-115">
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
                status
              });
            }}
          />
        </aside>
      </div>
      <LearningLogForm
        key={editingMoment?.learningMomentId ?? (addLog ? 'add-log-open' : 'add-log-closed')}
        children={children}
        moment={editingMoment}
        learningOutcomes={editingMoment ? learningOutcomes : activeLearningOutcomes}
        saving={logSaving}
        visible={addLog || Boolean(editingMoment)}
        onHide={handleLogFormHide}
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

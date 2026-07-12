import { useState } from 'react';
import { useHouseholdQuery } from '../../households/queries/household.queries';
import { useReadinessOutcomesQuery } from '../../readiness/queries/readinessOutcome.queries';
import { LearningHeader } from '../components/LearningHeader';
import { LearningLogForm } from '../components/LearningLogForm';
import { LearningMomentList } from '../components/LearningMomentList';
import { LearningTodaySummaryCard } from '../components/LearningTodaySummaryCard';
import { useCreateDailyLogMutation, useDailyLogsQuery, useDeleteDailyLogMutation } from '../queries/dailyLog.queries';
import type { LearningLogFormModel } from '../types/dailyLog.types';

const formatDateInputValue = (date: Date) => date.toISOString().slice(0, 10);

export const LearningPage = () => {
  const [addLog, setAddLog] = useState(false);

  const householdQuery = useHouseholdQuery();
  const learningOutcomesQuery = useReadinessOutcomesQuery();
  const dailyLogsQuery = useDailyLogsQuery();
  const createDailyLogMutation = useCreateDailyLogMutation();
  const deleteDailyLogMutation = useDeleteDailyLogMutation();

  const children = householdQuery.data?.children ?? [];
  const learningOutcomes = learningOutcomesQuery.data ?? [];
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

  return (
    <section aria-labelledby="learning-title" className="space-y-5">
      <LearningHeader
        onAddLog={() => {
          setAddLog(true);
        }}
      />

      <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_360px]">
        <LearningMomentList
          isDeleting={deleteDailyLogMutation.isPending}
          moments={latestMoments}
          onDeleteMoment={(dailyLogId) => {
            deleteDailyLogMutation.mutate(dailyLogId);
          }}
        />

        <aside className="space-y-5">
          <LearningTodaySummaryCard activityCount={todayActivityCount} readingCount={todayReadingCount} routineCount={todayRoutineCount} />
        </aside>
      </div>

      <LearningLogForm
        key={addLog ? 'add-log-open' : 'add-log-closed'}
        children={children}
        learningOutcomes={learningOutcomes}
        saving={createDailyLogMutation.isPending}
        visible={addLog}
        onHide={() => {
          setAddLog(false);
        }}
        onSave={(form) => {
          void handleSave(form);
        }}
      />
    </section>
  );
};

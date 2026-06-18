import type { DailyLogModel, DailyLogResponse } from '../types/dailyLog.types';

export const mapDailyLogResponseToModel = (response: DailyLogResponse): DailyLogModel => ({
  dailyLogId: response.dailyLogId,
  householdId: response.householdId,
  childId: response.childId,
  logDate: response.logDate,
  completedActivityCount: response.completedActivityCount,
  readingEntryCount: response.readingEntryCount,
  routineEntryCount: response.routineEntryCount
});

export const mapDailyLogResponsesToModels = (responses: DailyLogResponse[]): DailyLogModel[] => responses.map(mapDailyLogResponseToModel);

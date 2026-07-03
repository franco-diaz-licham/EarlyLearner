import type { DailyLogModel, DailyLogResponse } from '../types/dailyLog.types';

export const mapDailyLogResponseToModel = (response: DailyLogResponse): DailyLogModel => ({
  dailyLogId: response.dailyLogId,
  householdId: response.householdId,
  childId: response.childId,
  logDate: response.logDate,
  learningMomentCount: response.learningMomentCount
});

export const mapDailyLogResponsesToModels = (responses: DailyLogResponse[]): DailyLogModel[] => responses.map(mapDailyLogResponseToModel);

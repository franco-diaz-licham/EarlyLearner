import { apiClient } from '../../../shared/api/apiClient';
import { mapDailyLogResponseToModel, mapDailyLogResponsesToModels } from '../mappers/dailyLog.mapper';
import type { CreateDailyLogRequest, DailyLogModel, DailyLogResponse } from '../types/dailyLog.types';

const DAILY_LOGS_URL = '/daily-logs';

export const dailyLogService = {
  async list(): Promise<DailyLogModel[]> {
    const dailyLogs = await apiClient.getList<DailyLogResponse>(`${DAILY_LOGS_URL}/`);
    return mapDailyLogResponsesToModels(dailyLogs);
  },

  async get(dailyLogId: string): Promise<DailyLogModel> {
    const dailyLog = await apiClient.getSingle<DailyLogResponse>(`${DAILY_LOGS_URL}/${dailyLogId}`);
    return mapDailyLogResponseToModel(dailyLog);
  },

  async create(request: CreateDailyLogRequest): Promise<DailyLogModel> {
    const dailyLog = await apiClient.post<DailyLogResponse>(`${DAILY_LOGS_URL}/`, request);
    return mapDailyLogResponseToModel(dailyLog);
  },

  delete(dailyLogId: string): Promise<void> {
    return apiClient.delete(`${DAILY_LOGS_URL}/${dailyLogId}`);
  }
};

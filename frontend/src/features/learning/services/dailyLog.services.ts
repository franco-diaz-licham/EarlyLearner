import { apiClient } from '../../../shared/api/apiClient';
import type { ApiQueryParams, PaginatedResult } from '../../../shared/api/api.types';
import { mapDailyLogResponseToModel, mapDailyLogResponsesToModels, mapLearningMomentFeedResponseToModel } from '../mappers/dailyLog.mapper';
import type { CreateDailyLogRequest, DailyLogModel, DailyLogResponse, LearningMomentFeedModel, LearningMomentFeedResponse } from '../types/dailyLog.types';

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

  async listLearningMoments(params: ApiQueryParams): Promise<PaginatedResult<LearningMomentFeedModel>> {
    const result = await apiClient.getPaginatedList<LearningMomentFeedResponse>(`${DAILY_LOGS_URL}/learning-moments`, params);
    return {
      items: result.items.map(mapLearningMomentFeedResponseToModel),
      pagination: result.pagination
    };
  },

  async create(request: CreateDailyLogRequest): Promise<DailyLogModel> {
    const dailyLog = await apiClient.post<DailyLogResponse>(`${DAILY_LOGS_URL}/`, request);
    return mapDailyLogResponseToModel(dailyLog);
  },

  async deleteLearningMoment(dailyLogId: string, learningMomentId: string): Promise<void> {
    return apiClient.delete(`${DAILY_LOGS_URL}/${dailyLogId}/learning-moments/${learningMomentId}`);
  }
};

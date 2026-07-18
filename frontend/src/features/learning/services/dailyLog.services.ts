import { apiClient } from '../../../shared/api/apiClient';
import type { ApiQueryParams, PaginatedResult } from '../../../shared/api/api.types';
import type { CreateDailyLogRequest, DailyLogResponse, LearningMomentFeedResponse, UpdateLearningMomentRequest } from '../types/dailyLog.types';

const DAILY_LOGS_URL = '/daily-logs';

export const dailyLogService = {
  list(): Promise<DailyLogResponse[]> {
    return apiClient.getList<DailyLogResponse>(`${DAILY_LOGS_URL}/`);
  },

  get(dailyLogId: string): Promise<DailyLogResponse> {
    return apiClient.getSingle<DailyLogResponse>(`${DAILY_LOGS_URL}/${dailyLogId}`);
  },

  listLearningMoments(params: ApiQueryParams): Promise<PaginatedResult<LearningMomentFeedResponse>> {
    return apiClient.getPaginatedList<LearningMomentFeedResponse>(`${DAILY_LOGS_URL}/learning-moments`, params);
  },

  create(request: CreateDailyLogRequest): Promise<DailyLogResponse> {
    return apiClient.post<DailyLogResponse>(`${DAILY_LOGS_URL}/`, request);
  },

  updateLearningMoment(dailyLogId: string, learningMomentId: string, request: UpdateLearningMomentRequest): Promise<DailyLogResponse> {
    return apiClient.put<DailyLogResponse>(`${DAILY_LOGS_URL}/${dailyLogId}/learning-moments/${learningMomentId}`, request);
  },

  deleteLearningMoment(dailyLogId: string, learningMomentId: string): Promise<void> {
    return apiClient.delete(`${DAILY_LOGS_URL}/${dailyLogId}/learning-moments/${learningMomentId}`);
  }
};
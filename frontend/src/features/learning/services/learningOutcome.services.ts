import { apiClient } from '../../../shared/api/apiClient';
import type { CreateLearningOutcomeRequest, LearningOutcomeResponse, UpdateLearningOutcomeRequest, UpdateLearningOutcomeStatusRequest } from '../types/learningOutcome.types';

const LEARNING_OUTCOMES_URL = '/learning-outcomes';

export const learningOutcomeService = {
  list(): Promise<LearningOutcomeResponse[]> {
    return apiClient.getList<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/`);
  },

  get(learningOutcomeId: string): Promise<LearningOutcomeResponse> {
    return apiClient.getSingle<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/${learningOutcomeId}`);
  },

  create(request: CreateLearningOutcomeRequest): Promise<LearningOutcomeResponse> {
    return apiClient.post<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/`, request);
  },

  update(learningOutcomeId: string, request: UpdateLearningOutcomeRequest): Promise<LearningOutcomeResponse> {
    return apiClient.put<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/${learningOutcomeId}`, request);
  },

  updateStatus(learningOutcomeId: string, request: UpdateLearningOutcomeStatusRequest): Promise<LearningOutcomeResponse> {
    return apiClient.patch<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/${learningOutcomeId}/status`, request);
  },

  delete(learningOutcomeId: string): Promise<void> {
    return apiClient.delete(`${LEARNING_OUTCOMES_URL}/${learningOutcomeId}`);
  }
};
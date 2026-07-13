import { apiClient } from '../../../shared/api/apiClient';
import { mapLearningOutcomeResponseToModel, mapLearningOutcomeResponsesToModels } from '../mappers/learningOutcome.mapper';
import type { CreateLearningOutcomeRequest, LearningOutcomeModel, LearningOutcomeResponse, UpdateLearningOutcomeRequest } from '../types/learningOutcome.types';

const LEARNING_OUTCOMES_URL = '/learning-outcomes';

export const learningOutcomeService = {
  async list(): Promise<LearningOutcomeModel[]> {
    const learningOutcomes = await apiClient.getList<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/`);
    return mapLearningOutcomeResponsesToModels(learningOutcomes);
  },

  async get(learningOutcomeId: string): Promise<LearningOutcomeModel> {
    const learningOutcome = await apiClient.getSingle<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/${learningOutcomeId}`);
    return mapLearningOutcomeResponseToModel(learningOutcome);
  },

  async create(request: CreateLearningOutcomeRequest): Promise<LearningOutcomeModel> {
    const learningOutcome = await apiClient.post<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/`, request);
    return mapLearningOutcomeResponseToModel(learningOutcome);
  },

  async update(learningOutcomeId: string, request: UpdateLearningOutcomeRequest): Promise<LearningOutcomeModel> {
    const learningOutcome = await apiClient.put<LearningOutcomeResponse>(`${LEARNING_OUTCOMES_URL}/${learningOutcomeId}`, request);
    return mapLearningOutcomeResponseToModel(learningOutcome);
  },

  delete(learningOutcomeId: string): Promise<void> {
    return apiClient.delete(`${LEARNING_OUTCOMES_URL}/${learningOutcomeId}`);
  }
};

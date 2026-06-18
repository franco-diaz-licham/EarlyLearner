import { apiClient } from '../../../shared/api/apiClient';
import { mapLearningPlanResponseToModel, mapLearningPlanResponsesToModels } from '../mappers/learningPlan.mapper';
import type { CreateLearningPlanRequest, LearningPlanModel, LearningPlanResponse, UpdateLearningPlanRequest } from '../types/learningPlan.types';

const LEARNING_PLANS_URL = '/learning-plans';

export const learningPlanService = {
  async list(householdId: string): Promise<LearningPlanModel[]> {
    const learningPlans = await apiClient.getList<LearningPlanResponse>(`${LEARNING_PLANS_URL}/`, { householdId });
    return mapLearningPlanResponsesToModels(learningPlans);
  },

  async get(learningPlanId: string): Promise<LearningPlanModel> {
    const learningPlan = await apiClient.getSingle<LearningPlanResponse>(`${LEARNING_PLANS_URL}/${learningPlanId}`);
    return mapLearningPlanResponseToModel(learningPlan);
  },

  async create(request: CreateLearningPlanRequest): Promise<LearningPlanModel> {
    const learningPlan = await apiClient.post<LearningPlanResponse>(`${LEARNING_PLANS_URL}/`, request);
    return mapLearningPlanResponseToModel(learningPlan);
  },

  async update(learningPlanId: string, request: UpdateLearningPlanRequest): Promise<LearningPlanModel> {
    const learningPlan = await apiClient.put<LearningPlanResponse>(`${LEARNING_PLANS_URL}/${learningPlanId}`, request);
    return mapLearningPlanResponseToModel(learningPlan);
  },

  delete(learningPlanId: string): Promise<void> {
    return apiClient.delete(`${LEARNING_PLANS_URL}/${learningPlanId}`);
  }
};

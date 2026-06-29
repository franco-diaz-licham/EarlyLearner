import { apiClient } from '../../../shared/api/apiClient';
import { mapGoalResponseToModel, mapGoalResponsesToModels } from '../mappers/goal.mapper';
import type { CreateGoalRequest, GoalModel, GoalResponse, UpdateGoalRequest } from '../types/goal.types';

const GOALS_URL = '/goals';

export const goalService = {
  async list(): Promise<GoalModel[]> {
    const goals = await apiClient.getList<GoalResponse>(`${GOALS_URL}/`);
    return mapGoalResponsesToModels(goals);
  },

  async get(goalId: string): Promise<GoalModel> {
    const goal = await apiClient.getSingle<GoalResponse>(`${GOALS_URL}/${goalId}`);
    return mapGoalResponseToModel(goal);
  },

  async create(request: CreateGoalRequest): Promise<GoalModel> {
    const goal = await apiClient.post<GoalResponse>(`${GOALS_URL}/`, request);
    return mapGoalResponseToModel(goal);
  },

  async update(goalId: string, request: UpdateGoalRequest): Promise<GoalModel> {
    const goal = await apiClient.put<GoalResponse>(`${GOALS_URL}/${goalId}`, request);
    return mapGoalResponseToModel(goal);
  },

  delete(goalId: string): Promise<void> {
    return apiClient.delete(`${GOALS_URL}/${goalId}`);
  }
};

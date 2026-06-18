import type { GoalModel, GoalResponse } from '../types/goal.types';

export const mapGoalResponseToModel = (response: GoalResponse): GoalModel => ({
  goalId: response.goalId,
  householdId: response.householdId,
  childId: response.childId,
  title: response.title,
  type: response.type,
  status: response.status,
  startDate: response.startDate,
  endDate: response.endDate,
  readinessOutcomeIds: response.readinessOutcomeIds
});

export const mapGoalResponsesToModels = (responses: GoalResponse[]): GoalModel[] => responses.map(mapGoalResponseToModel);

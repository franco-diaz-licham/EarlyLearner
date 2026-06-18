import type { LearningPlanModel, LearningPlanResponse } from '../types/learningPlan.types';

export const mapLearningPlanResponseToModel = (response: LearningPlanResponse): LearningPlanModel => ({
  learningPlanId: response.learningPlanId,
  householdId: response.householdId,
  childId: response.childId,
  startDate: response.startDate,
  endDate: response.endDate,
  focus: response.focus
});

export const mapLearningPlanResponsesToModels = (responses: LearningPlanResponse[]): LearningPlanModel[] => responses.map(mapLearningPlanResponseToModel);

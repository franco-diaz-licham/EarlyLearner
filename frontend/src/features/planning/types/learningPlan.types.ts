export interface LearningPlanResponse {
  learningPlanId: string;
  householdId: string;
  childId: string;
  startDate: string;
  endDate: string;
  focus: string;
}

export interface LearningPlanModel {
  learningPlanId: string;
  householdId: string;
  childId: string;
  startDate: string;
  endDate: string;
  focus: string;
}

export interface CreateLearningPlanRequest {
  householdId: string;
  childId: string;
  startDate: string;
  endDate: string;
  focus: string;
}

export interface UpdateLearningPlanRequest {
  startDate: string;
  endDate: string;
  focus: string;
}

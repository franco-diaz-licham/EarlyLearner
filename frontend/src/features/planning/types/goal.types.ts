export const GoalType = {
  ShortTerm: 1,
  LongTerm: 2
} as const;

export type GoalType = (typeof GoalType)[keyof typeof GoalType];

export const GoalStatus = {
  Active: 1,
  Completed: 2,
  Archived: 3
} as const;

export type GoalStatus = (typeof GoalStatus)[keyof typeof GoalStatus];

export interface GoalResponse {
  goalId: string;
  householdId: string;
  childId: string;
  title: string;
  type: GoalType;
  status: GoalStatus;
  startDate: string;
  endDate: string;
  readinessOutcomeIds: string[];
}

export interface GoalModel {
  goalId: string;
  householdId: string;
  childId: string;
  title: string;
  type: GoalType;
  status: GoalStatus;
  startDate: string;
  endDate: string;
  readinessOutcomeIds: string[];
}

export interface CreateGoalRequest {
  householdId: string;
  childId: string;
  title: string;
  type: GoalType;
  startDate: string;
  endDate: string;
  readinessOutcomeIds: string[];
}

export interface UpdateGoalRequest {
  title: string;
}

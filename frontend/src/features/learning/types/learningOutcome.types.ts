export const LearningOutcomeStatus = {
  Active: 'active',
  Inactive: 'inactive',
  Archived: 'archived'
} as const;

export type LearningOutcomeStatus = (typeof LearningOutcomeStatus)[keyof typeof LearningOutcomeStatus];

export interface LearningOutcomeResponse {
  learningOutcomeId: string;
  code: string;
  name: string;
  description: string;
  category: string;
  sortOrder: number;
  status: LearningOutcomeStatus;
}

export interface LearningOutcomeModel {
  learningOutcomeId: string;
  code: string;
  name: string;
  description: string;
  category: string;
  sortOrder: number;
  status: LearningOutcomeStatus;
}

export interface CreateLearningOutcomeRequest {
  code: string;
  name: string;
  description: string;
  category: string;
  sortOrder: number;
}

export interface UpdateLearningOutcomeRequest {
  name: string;
  description: string;
  category: string;
  sortOrder: number;
}

export interface UpdateLearningOutcomeStatusRequest {
  status: LearningOutcomeStatus;
}

export const ReadinessOutcomeStatus = {
  Active: 1,
  Inactive: 2,
  Archived: 3
} as const;

export type ReadinessOutcomeStatus = (typeof ReadinessOutcomeStatus)[keyof typeof ReadinessOutcomeStatus];

export interface ReadinessOutcomeResponse {
  readinessOutcomeId: string;
  code: string;
  name: string;
  description: string;
  category: string;
  sortOrder: number;
  status: ReadinessOutcomeStatus;
}

export interface ReadinessOutcomeModel {
  readinessOutcomeId: string;
  code: string;
  name: string;
  description: string;
  category: string;
  sortOrder: number;
  status: ReadinessOutcomeStatus;
}

export interface CreateReadinessOutcomeRequest {
  code: string;
  name: string;
  description: string;
  category: string;
  sortOrder: number;
}

export interface UpdateReadinessOutcomeRequest {
  name: string;
  description: string;
  category: string;
  sortOrder: number;
}

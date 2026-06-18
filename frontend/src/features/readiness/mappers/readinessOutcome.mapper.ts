import type { ReadinessOutcomeModel, ReadinessOutcomeResponse } from '../types/readinessOutcome.types';

export const mapReadinessOutcomeResponseToModel = (response: ReadinessOutcomeResponse): ReadinessOutcomeModel => ({
  readinessOutcomeId: response.readinessOutcomeId,
  code: response.code,
  name: response.name,
  description: response.description,
  category: response.category,
  sortOrder: response.sortOrder,
  status: response.status
});

export const mapReadinessOutcomeResponsesToModels = (responses: ReadinessOutcomeResponse[]): ReadinessOutcomeModel[] => responses.map(mapReadinessOutcomeResponseToModel);

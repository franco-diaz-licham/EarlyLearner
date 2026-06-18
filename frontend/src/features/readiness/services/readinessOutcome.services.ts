import { apiClient } from '../../../shared/api/apiClient';
import { mapReadinessOutcomeResponseToModel, mapReadinessOutcomeResponsesToModels } from '../mappers/readinessOutcome.mapper';
import type { CreateReadinessOutcomeRequest, ReadinessOutcomeModel, ReadinessOutcomeResponse, UpdateReadinessOutcomeRequest } from '../types/readinessOutcome.types';

const READINESS_OUTCOMES_URL = '/readiness-outcomes';

export const readinessOutcomeService = {
  async list(): Promise<ReadinessOutcomeModel[]> {
    const readinessOutcomes = await apiClient.getList<ReadinessOutcomeResponse>(`${READINESS_OUTCOMES_URL}/`);
    return mapReadinessOutcomeResponsesToModels(readinessOutcomes);
  },

  async get(readinessOutcomeId: string): Promise<ReadinessOutcomeModel> {
    const readinessOutcome = await apiClient.getSingle<ReadinessOutcomeResponse>(`${READINESS_OUTCOMES_URL}/${readinessOutcomeId}`);
    return mapReadinessOutcomeResponseToModel(readinessOutcome);
  },

  async create(request: CreateReadinessOutcomeRequest): Promise<ReadinessOutcomeModel> {
    const readinessOutcome = await apiClient.post<ReadinessOutcomeResponse>(`${READINESS_OUTCOMES_URL}/`, request);
    return mapReadinessOutcomeResponseToModel(readinessOutcome);
  },

  async update(readinessOutcomeId: string, request: UpdateReadinessOutcomeRequest): Promise<ReadinessOutcomeModel> {
    const readinessOutcome = await apiClient.put<ReadinessOutcomeResponse>(`${READINESS_OUTCOMES_URL}/${readinessOutcomeId}`, request);
    return mapReadinessOutcomeResponseToModel(readinessOutcome);
  },

  delete(readinessOutcomeId: string): Promise<void> {
    return apiClient.delete(`${READINESS_OUTCOMES_URL}/${readinessOutcomeId}`);
  }
};

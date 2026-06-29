import { apiClient } from '../../../shared/api/apiClient';
import { mapReadinessProfileResponseToModel, mapReadinessProfileResponsesToModels } from '../mappers/readinessProfile.mapper';
import type { CreateReadinessProfileRequest, ReadinessProfileModel, ReadinessProfileResponse } from '../types/readinessProfile.types';

const READINESS_PROFILES_URL = '/readiness-profiles';

export const readinessProfileService = {
  async list(): Promise<ReadinessProfileModel[]> {
    const readinessProfiles = await apiClient.getList<ReadinessProfileResponse>(`${READINESS_PROFILES_URL}/`);
    return mapReadinessProfileResponsesToModels(readinessProfiles);
  },

  async get(readinessProfileId: string): Promise<ReadinessProfileModel> {
    const readinessProfile = await apiClient.getSingle<ReadinessProfileResponse>(`${READINESS_PROFILES_URL}/${readinessProfileId}`);
    return mapReadinessProfileResponseToModel(readinessProfile);
  },

  async create(request: CreateReadinessProfileRequest): Promise<ReadinessProfileModel> {
    const readinessProfile = await apiClient.post<ReadinessProfileResponse>(`${READINESS_PROFILES_URL}/`, request);
    return mapReadinessProfileResponseToModel(readinessProfile);
  },

  delete(readinessProfileId: string): Promise<void> {
    return apiClient.delete(`${READINESS_PROFILES_URL}/${readinessProfileId}`);
  }
};

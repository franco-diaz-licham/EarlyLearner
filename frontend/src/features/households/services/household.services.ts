import { apiClient } from '../../../shared/api/apiClient';
import { fileUploadService } from '../../../shared/services/fileUpload.service';
import type { AddHouseholdChildRequest, HouseholdResponse, InviteHouseholdCarerRequest, UpdateHouseholdChildRequest, UpdateHouseholdRequest } from '../types/household.api.types';

const HOUSEHOLDS_URL = '/households';

export const householdService = {
  async list(): Promise<HouseholdResponse[]> {
    return apiClient.getList<HouseholdResponse>(HOUSEHOLDS_URL);
  },

  async get(householdId: string): Promise<HouseholdResponse> {
    return apiClient.getSingle<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}`);
  },

  async update(householdId: string, request: UpdateHouseholdRequest): Promise<HouseholdResponse> {
    return apiClient.put<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}`, request);
  },

  async inviteCarer(householdId: string, request: InviteHouseholdCarerRequest): Promise<HouseholdResponse> {
    return apiClient.post<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/carer-invitations`, request);
  },

  async removeCarer(householdId: string, carerId: string): Promise<HouseholdResponse> {
    return apiClient.deleteResult<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/carers/${carerId}`);
  },

  async addChild(householdId: string, request: AddHouseholdChildRequest): Promise<HouseholdResponse> {
    return apiClient.post<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/children`, request);
  },

  async updateChild(householdId: string, childId: string, request: UpdateHouseholdChildRequest): Promise<HouseholdResponse> {
    return apiClient.put<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/children/${childId}`, request);
  },

  async uploadChildAvatar(householdId: string, childId: string, file: File): Promise<HouseholdResponse> {
    return fileUploadService.upload<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/children/${childId}/avatar`, { file });
  },

  async removeChild(householdId: string, childId: string): Promise<HouseholdResponse> {
    return apiClient.deleteResult<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/children/${childId}`);
  }
};

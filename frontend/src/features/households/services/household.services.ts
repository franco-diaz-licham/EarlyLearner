import { apiClient } from '../../../shared/api/apiClient';
import type { AddHouseholdChildRequest, HouseholdResponse, InviteHouseholdCarerRequest, UpdateHouseholdChildRequest, UpdateHouseholdRequest } from '../types/household.api.types';

const HOUSEHOLDS_URL = '/households';

export const householdService = {
  async list(): Promise<HouseholdResponse[]> {
    return apiClient.getList<HouseholdResponse>(HOUSEHOLDS_URL);
  },

  async get(): Promise<HouseholdResponse> {
    return apiClient.getSingle<HouseholdResponse>(`${HOUSEHOLDS_URL}/current`);
  },

  async update(request: UpdateHouseholdRequest): Promise<HouseholdResponse> {
    return apiClient.put<HouseholdResponse>(HOUSEHOLDS_URL, request);
  },

  async inviteCarer(request: InviteHouseholdCarerRequest): Promise<HouseholdResponse> {
    return apiClient.post<HouseholdResponse>(`${HOUSEHOLDS_URL}/carer-invitations`, request);
  },

  async removeCarer(carerId: string): Promise<HouseholdResponse> {
    return apiClient.deleteResult<HouseholdResponse>(`${HOUSEHOLDS_URL}/carers/${carerId}`);
  },

  async addChild(request: AddHouseholdChildRequest): Promise<HouseholdResponse> {
    return apiClient.post<HouseholdResponse>(`${HOUSEHOLDS_URL}/children`, request);
  },

  async updateChild(childId: string, request: UpdateHouseholdChildRequest): Promise<HouseholdResponse> {
    return apiClient.put<HouseholdResponse>(`${HOUSEHOLDS_URL}/children/${childId}`, request);
  },

  async removeChild(childId: string): Promise<HouseholdResponse> {
    return apiClient.deleteResult<HouseholdResponse>(`${HOUSEHOLDS_URL}/children/${childId}`);
  }
};

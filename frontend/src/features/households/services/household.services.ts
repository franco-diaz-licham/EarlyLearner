import { apiClient } from '../../../shared/api/apiClient';
import type { AddHouseholdChildRequest, HouseholdResponse, InviteHouseholdCarerRequest, UpdateHouseholdRequest } from '../types/household.api.types';

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

  async removeChild(householdId: string, childId: string): Promise<HouseholdResponse> {
    return apiClient.deleteResult<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/children/${childId}`);
  }
};

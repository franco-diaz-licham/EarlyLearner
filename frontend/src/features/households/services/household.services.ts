import { apiClient } from '../../../shared/api/apiClient';
import { mapHouseholdResponseToModel, mapHouseholdResponsesToModels } from '../mappers/household.mapper';
import type { AddHouseholdChildRequest, HouseholdModel, HouseholdResponse, InviteHouseholdCarerRequest, UpdateHouseholdRequest } from '../types/household.types';

const HOUSEHOLDS_URL = '/households';

export const householdService = {
  async list(): Promise<HouseholdModel[]> {
    const households = await apiClient.getList<HouseholdResponse>(HOUSEHOLDS_URL);
    return mapHouseholdResponsesToModels(households);
  },

  async get(householdId: string): Promise<HouseholdModel> {
    const household = await apiClient.getSingle<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}`);
    return mapHouseholdResponseToModel(household);
  },

  async update(householdId: string, request: UpdateHouseholdRequest): Promise<HouseholdModel> {
    const household = await apiClient.put<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}`, request);
    return mapHouseholdResponseToModel(household);
  },

  async inviteCarer(householdId: string, request: InviteHouseholdCarerRequest): Promise<HouseholdModel> {
    const household = await apiClient.post<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/carer-invitations`, request);
    return mapHouseholdResponseToModel(household);
  },

  async removeCarer(householdId: string, carerId: string): Promise<HouseholdModel> {
    const household = await apiClient.deleteResult<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/carers/${carerId}`);
    return mapHouseholdResponseToModel(household);
  },

  async addChild(householdId: string, request: AddHouseholdChildRequest): Promise<HouseholdModel> {
    const household = await apiClient.post<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/children`, request);
    return mapHouseholdResponseToModel(household);
  },

  async removeChild(householdId: string, childId: string): Promise<HouseholdModel> {
    const household = await apiClient.deleteResult<HouseholdResponse>(`${HOUSEHOLDS_URL}/${householdId}/children/${childId}`);
    return mapHouseholdResponseToModel(household);
  },

  delete(householdId: string): Promise<void> {
    return apiClient.delete(`${HOUSEHOLDS_URL}/${householdId}`);
  }
};

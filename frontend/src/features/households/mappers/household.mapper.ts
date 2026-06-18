import type { HouseholdModel, HouseholdResponse } from '../types/household.types';

export const mapHouseholdResponseToModel = (response: HouseholdResponse): HouseholdModel => ({
  householdId: response.householdId,
  name: response.name
});

export const mapHouseholdResponsesToModels = (responses: HouseholdResponse[]): HouseholdModel[] => responses.map(mapHouseholdResponseToModel);

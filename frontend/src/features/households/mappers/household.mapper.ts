import type { HouseholdModel, HouseholdResponse } from '../types/household.types';

export const mapHouseholdResponseToModel = (response: HouseholdResponse): HouseholdModel => ({
  id: response.id,
  name: response.name
});

export const mapHouseholdResponsesToModels = (responses: HouseholdResponse[]): HouseholdModel[] => responses.map(mapHouseholdResponseToModel);

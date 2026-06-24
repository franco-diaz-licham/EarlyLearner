import type { HouseholdModel, HouseholdResponse } from '../types/household.types';

export const mapHouseholdResponseToModel = (response: HouseholdResponse): HouseholdModel => ({
  id: response.id,
  name: response.name,
  carers: response.carers,
  children: response.children,
  invitations: response.invitations
});

export const mapHouseholdResponsesToModels = (responses: HouseholdResponse[]): HouseholdModel[] => responses.map(mapHouseholdResponseToModel);

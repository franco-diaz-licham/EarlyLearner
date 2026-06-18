import type { ReadinessProfileModel, ReadinessProfileResponse } from '../types/readinessProfile.types';

export const mapReadinessProfileResponseToModel = (response: ReadinessProfileResponse): ReadinessProfileModel => ({
  readinessProfileId: response.readinessProfileId,
  householdId: response.householdId,
  childId: response.childId,
  readinessOutcomeIds: response.readinessOutcomeIds
});

export const mapReadinessProfileResponsesToModels = (responses: ReadinessProfileResponse[]): ReadinessProfileModel[] => responses.map(mapReadinessProfileResponseToModel);

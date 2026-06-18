export interface ReadinessProfileResponse {
  readinessProfileId: string;
  householdId: string;
  childId: string;
  readinessOutcomeIds: string[];
}

export interface ReadinessProfileModel {
  readinessProfileId: string;
  householdId: string;
  childId: string;
  readinessOutcomeIds: string[];
}

export interface CreateReadinessProfileRequest {
  householdId: string;
  childId: string;
  readinessOutcomeIds: string[];
}

export interface HouseholdResponse {
  householdId: string;
  name: string;
}

export interface HouseholdModel {
  householdId: string;
  name: string;
}

export interface CreateHouseholdRequest {
  name: string;
  ownerUserId: string;
  ownerFirstName: string;
  ownerLastName: string;
}

export interface UpdateHouseholdRequest {
  name: string;
}

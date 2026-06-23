export interface HouseholdResponse {
  id: string;
  name: string;
  carers: CarerResponse[];
  children: ChildResponse[];
}

export interface HouseholdModel {
  id: string;
  name: string;
}

export interface CreateHouseholdRequest {
  name: string;
}

export interface UpdateHouseholdRequest {
  name: string;
}

export type HouseholdRole = 2 | 3;

export interface InviteHouseholdCarerRequest {
  email: string;
  firstName: string;
  lastName: string;
  role: HouseholdRole;
}

export interface AddHouseholdChildRequest {
  givenName: string;
  dateOfBirth: string;
}

export interface CarerResponse {
  id: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  accountStatus: string;
}

export interface ChildResponse {
  id: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}

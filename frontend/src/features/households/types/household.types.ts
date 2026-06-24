export interface HouseholdResponse {
  id: string;
  name: string;
  carers: CarerResponse[];
  children: ChildResponse[];
  invitations: HouseholdInvitationResponse[];
}

export interface HouseholdModel {
  id: string;
  name: string;
  carers: CarerResponse[];
  children: ChildResponse[];
  invitations: HouseholdInvitationResponse[];
}

export interface UpdateHouseholdRequest {
  name: string;
}

export type HouseholdRole = 2 | 3;

export interface InviteHouseholdCarerRequest {
  email: string;
  role: HouseholdRole;
}

export interface AddHouseholdChildRequest {
  firstName: string;
  lastName: string;
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

export interface HouseholdInvitationResponse {
  id: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  role: string;
  status: string;
  invitedAt: string;
  expiresAt: string;
}

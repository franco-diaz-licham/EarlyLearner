import type { HouseholdRole } from './household.types';

export interface HouseholdResponse {
  id: string;
  name: string;
  carers: CarerResponse[];
  children: ChildResponse[];
  invitations: HouseholdInvitationResponse[];
}

export interface UpdateHouseholdRequest {
  name: string;
}

export interface InviteHouseholdCarerRequest {
  email: string;
  role: HouseholdRole;
}

export interface AddHouseholdChildRequest {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}

export interface UpdateHouseholdChildRequest {
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

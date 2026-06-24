export interface HouseholdModel {
  id: string;
  name: string;
  carers: CarerModel[];
  children: ChildModel[];
  invitations: HouseholdInvitationModel[];
}

export interface CarerModel {
  id: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  accountStatus: string;
}

export interface ChildModel {
  id: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}

export interface HouseholdInvitationModel {
  id: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  role: string;
  status: string;
  invitedAt: string;
  expiresAt: string;
}

export interface RenameHouseholdForm {
  name: string;
}

export interface InviteCarerForm {
  email: string;
  role: HouseholdRole;
}

export interface AddChildForm {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}

export type HouseholdRole = 'caregiver' | 'viewer';

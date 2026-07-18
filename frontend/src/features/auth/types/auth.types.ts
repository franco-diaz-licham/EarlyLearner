/** App authenticated account for user. */
export interface AuthAccount {
  id: string;
  name?: string;
  username: string;
}

export interface AuthSessionResponse {
  fullName: string;
  userId: string;
  householdId: string;
  accessibleHouseholdIds: string[];
  status: string;
  carerId: string | null;
}

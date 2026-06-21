export interface AuthAccount {
  id: string;
  name?: string;
  username: string;
}

export interface AuthRedirectResult {
  account: AuthAccount | null;
}

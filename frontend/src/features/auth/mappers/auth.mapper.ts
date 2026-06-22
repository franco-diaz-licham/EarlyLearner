import type { AuthAccount } from '../types/auth.types';
import type { MsalAccount } from '../types/msal.types';

export const toAuthAccount = (account: MsalAccount | null): AuthAccount | null => {
  if (!account) return null;

  return {
    id: account.homeAccountId,
    name: account.name,
    username: account.username
  };
};

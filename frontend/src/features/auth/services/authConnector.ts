import { BrowserAuthError, InteractionRequiredAuthError, PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from '../config/authConfig';
import { appConfig } from '../../../shared/config/appConfig';
import type { AuthAccount, AuthRedirectResult } from '../types/auth.types';

interface TokenOptions {
  allowRedirect: boolean;
  beforeRedirect: () => void;
}

interface AuthRequest {
  scopes: string[];
  prompt?: string;
}

const authRequest: AuthRequest = {
  scopes: [appConfig.entraApiScope].filter(Boolean),
  prompt: 'select_account'
};

interface MsalAccount {
  homeAccountId: string;
  environment: string;
  tenantId: string;
  localAccountId: string;
  name?: string;
  username: string;
}

interface MsalAuthResult {
  accessToken: string;
  account: MsalAccount;
}

export interface MsalClient {
  initialize: () => Promise<void>;
  handleRedirectPromise: () => Promise<MsalAuthResult | null>;
  getActiveAccount: () => MsalAccount | null;
  getAllAccounts: () => MsalAccount[];
  setActiveAccount: (account: MsalAccount | null) => void;
  loginRedirect: (request: AuthRequest & { redirectStartPage?: string }) => Promise<void>;
  logoutRedirect: (request: { account?: MsalAccount }) => Promise<void>;
  acquireTokenSilent: (request: AuthRequest & { account: MsalAccount }) => Promise<MsalAuthResult>;
  acquireTokenRedirect: (request: AuthRequest) => Promise<void>;
}

let msalInstance: MsalClient | null = null;

const getMsalInstance = (): MsalClient => {
  if (msalInstance === null) throw new Error('Authentication has not been initialised.');
  return msalInstance;
};

const toAuthAccount = (account: MsalAccount | null): AuthAccount | null => {
  if (!account) return null;

  return {
    id: account.homeAccountId,
    name: account.name,
    username: account.username
  };
};

const getAuthErrorText = (err: unknown): string => {
  if (err instanceof Error) return `${err.name} ${err.message}`;
  if (!err || typeof err !== 'object') return String(err);

  const payload = err as Record<string, unknown>;
  return [payload.name, payload.errorCode, payload.errorMessage, payload.message].filter((value): value is string => typeof value === 'string').join(' ');
};

const syncFirstAccountAsActive = () => {
  const instance = getMsalInstance();
  if (instance.getActiveAccount()) return;

  const accounts = instance.getAllAccounts();
  if (accounts.length > 0) instance.setActiveAccount(accounts[0]);
};

export const authConnector = {
  async initialize(): Promise<void> {
    msalInstance ??= new PublicClientApplication(msalConfig);
    await getMsalInstance().initialize();
    syncFirstAccountAsActive();
  },

  async handleRedirect(): Promise<AuthRedirectResult | null> {
    const instance = getMsalInstance();
    const result = await instance.handleRedirectPromise();
    if (result === null) return null;

    instance.setActiveAccount(result.account);
    return { account: toAuthAccount(result.account) };
  },

  getCurrentAccount(): AuthAccount | null {
    syncFirstAccountAsActive();
    return toAuthAccount(getMsalInstance().getActiveAccount());
  },

  login(): Promise<void> {
    const instance = getMsalInstance();
    instance.setActiveAccount(null);
    return instance.loginRedirect({ ...authRequest, redirectStartPage: '/' });
  },

  logout(): Promise<void> {
    const instance = getMsalInstance();
    return instance.logoutRedirect({ account: instance.getActiveAccount() ?? undefined });
  },

  async getToken({ allowRedirect, beforeRedirect }: TokenOptions): Promise<string | null> {
    syncFirstAccountAsActive();
    const instance = getMsalInstance();
    const account = instance.getActiveAccount();
    if (!account) return null;

    try {
      const result = await instance.acquireTokenSilent({
        ...authRequest,
        account
      });
      return result.accessToken;
    } catch (err) {
      if (err instanceof InteractionRequiredAuthError && allowRedirect) {
        beforeRedirect();
        await instance.acquireTokenRedirect(authRequest);
      }

      return null;
    }
  },

  isInteractionInProgressError(err: unknown): boolean {
    return err instanceof BrowserAuthError && err.errorCode === 'interaction_in_progress';
  },

  isUnsupportedTenantAccessError(err: unknown): boolean {
    const text = getAuthErrorText(err).toLowerCase();
    return text.includes('aadsts650052') || (text.includes('invalid_client') && text.includes('lacks a service principal'));
  }
};

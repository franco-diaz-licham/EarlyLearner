import { BrowserAuthError, InteractionRequiredAuthError, PublicClientApplication, type IPublicClientApplication } from '@azure/msal-browser';
import { appConfig } from '../../../shared/config/appConfig';
import type { AuthAccount, AuthRedirectResult } from '../types/auth.types';
import { msalConfig, type AuthConnector, type MsalAuthRequest, type MsalTokenOptions } from '../types/msal.types';
import { toAuthAccount } from '../mappers/auth.mapper';

/**
 * Microsoft request for forcing users to select an account.
 * This is important since many users have microsoft logins already.
 */
const loginRequest: MsalAuthRequest = {
  scopes: [appConfig.entraApiScope].filter(Boolean),
  prompt: 'select_account'
};

/** Allow users to create an new account. */
const createAccountRequest: MsalAuthRequest = {
  scopes: [appConfig.entraApiScope].filter(Boolean),
  prompt: 'create'
};

/** Memoise the configured client at the module level so that the same instance survives the entirity of the app. */
let msalInstance: IPublicClientApplication | null = null;

const getMsalInstance = (): IPublicClientApplication => {
  if (msalInstance === null) throw new Error('Authentication has not been initialised.');
  return msalInstance;
};

const getAuthErrorText = (err: unknown): string => {
  if (err instanceof Error) return `${err.name} ${err.message}`;
  if (!err || typeof err !== 'object') return String(err);

  const payload = err as Record<string, unknown>;
  return [payload.name, payload.errorCode, payload.errorMessage, payload.message].filter((value): value is string => typeof value === 'string').join(' ');
};

/** MS authentication flow connector. */
export const authConnector: AuthConnector = {
  async initialize(): Promise<void> {
    msalInstance ??= new PublicClientApplication(msalConfig);
    const instance = getMsalInstance();
    await instance.initialize();
  },

  async handleRedirect(): Promise<AuthRedirectResult | null> {
    const instance = getMsalInstance();
    const result = await instance.handleRedirectPromise();
    if (result === null) return null;

    instance.setActiveAccount(result.account);
    return { account: toAuthAccount(result.account) };
  },

  getCurrentAccount(): AuthAccount | null {
    const instance = getMsalInstance();
    return toAuthAccount(instance.getActiveAccount());
  },

  login(redirectStartPage = window.location.href): Promise<void> {
    const instance = getMsalInstance();
    instance.setActiveAccount(null);
    return instance.loginRedirect({ ...loginRequest, redirectStartPage });
  },

  createAccount(redirectStartPage = window.location.href): Promise<void> {
    const instance = getMsalInstance();
    instance.setActiveAccount(null);
    return instance.loginRedirect({ ...createAccountRequest, redirectStartPage });
  },

  logout(): Promise<void> {
    const instance = getMsalInstance();
    return instance.logoutRedirect({ account: instance.getActiveAccount() ?? undefined, postLogoutRedirectUri: '/' });
  },

  async getAccessToken({ allowRedirect, beforeRedirect }: MsalTokenOptions): Promise<string | null> {
    const instance = getMsalInstance();
    const account = instance.getActiveAccount();
    if (!account) return null;

    try {
      const result = await instance.acquireTokenSilent({
        ...loginRequest,
        account
      });
      return result.accessToken;
    } catch (err) {
      if (err instanceof InteractionRequiredAuthError && allowRedirect) {
        beforeRedirect();
        await instance.acquireTokenRedirect(loginRequest);
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

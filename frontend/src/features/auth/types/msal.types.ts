import type { Configuration } from '@azure/msal-browser';
import { appConfig } from '../../../shared/config/appConfig';
import type { AuthAccount } from './auth.types';

/** Allows us to define a redirect so that we can bring the user back from where they logged in from */
export interface MsalTokenOptions {
  allowRedirect: boolean;
  beforeRedirect?: () => void;
}

/** Allows to determine the type of flow we want the user to udnergo. */
export interface MsalAuthRequest {
  scopes: string[];
  prompt?: string;
}

/** Claims from the ConnectID received from MS. */
export interface MsalAccount {
  homeAccountId: string;
  environment: string;
  tenantId: string;
  localAccountId: string;
  name?: string;
  username: string;
}

/** Final combined response received from MS.
 * It contains both the ConnectID defining user information
 * and the access_token that will be used to send requests to the API.
 */
export interface MsalAuthResult {
  accessToken: string;
  account: MsalAccount;
}

/** Global MSAL authentication configuration model. */
export const msalConfig: Configuration = {
  auth: {
    clientId: appConfig.entraClientId,
    authority: appConfig.entraAuthority,
    redirectUri: window.location.origin,
    postLogoutRedirectUri: window.location.origin
  },
  cache: {
    cacheLocation: 'sessionStorage'
  }
};

export interface AuthConnector {
  /**
   * Initialises the MSAL client and restores an existing cached account when available.
   *
   * This must run before any login, logout, redirect handling, or token acquisition
   * calls. If MSAL already has cached accounts, the first account is selected as
   * active so the app can resume an existing browser session.
   */
  initialize: () => Promise<void>;

  /**
   * Completes the Microsoft redirect flow and records the returned account as active.
   *
   * MSAL can cache multiple signed-in accounts in the browser, such as a personal
   * account and a work account. Setting the active account ensures later token
   * requests use the account that just completed authentication.
   *
   * @returns The authenticated account from the redirect response, or `null` when there is no redirect response to process.
   */
  handleRedirect: () => Promise<void>;

  /**
   * Returns the currently active Microsoft account.
   *
   * If MSAL has cached accounts but no active account has been selected yet, this
   * selects the first cached account before mapping it into the app auth model.
   *
   * @returns The active account, or `null` when no account is signed in.
   */
  getCurrentAccount: () => AuthAccount | null;

  /**
   * Starts the Microsoft login redirect flow.
   *
   * Clears the active account first so MSAL prompts the user to choose which
   * account to use instead of silently reusing a previous active account.
   */
  login: (redirectStartPage?: string) => Promise<void>;

  /**
   * Starts the Microsoft account creation redirect flow.
   *
   * Uses the Microsoft identity platform `prompt: 'create'` hint. Account creation
   * only works when the configured tenant/application supports self-service sign-up.
   */
  createAccount: (redirectStartPage?: string) => Promise<void>;

  /**
   * Starts the Microsoft logout redirect flow for the active account.
   *
   * When an active account exists, MSAL signs that account out and then redirects
   * back to the configured post-logout URI.
   */
  logout: () => Promise<void>;

  /**
   * Gets an access token for the active Microsoft account.
   *
   * Attempts silent token acquisition first. If Microsoft requires user
   * interaction and redirects are allowed, this starts an interactive token
   * redirect after calling `beforeRedirect`.
   *
   * @param options Token acquisition options.
   * @param options.allowRedirect Whether an interactive redirect may be started when silent acquisition fails.
   * @param options.beforeRedirect Callback invoked immediately before starting an interactive token redirect.
   * @returns The access token when one is available; otherwise `null`.
   */
  getAccessToken: (options: MsalTokenOptions) => Promise<string | null>;

  /**
   * Ensures the authenticated Microsoft account has a local application session.
   *
   * This is called once during app auth initialisation after MSAL has restored or
   * completed login. The backend may create/update the local user and default
   * household here.
   */
  ensureSession: () => Promise<void>;

  /**
   * Checks whether an auth error indicates another MSAL interaction is already running.
   *
   * MSAL allows only one interactive operation at a time. This helper lets callers
   * ignore duplicate login/logout attempts caused by repeated clicks or overlapping
   * auth flows.
   *
   * @param err The caught error to inspect.
   * @returns `true` when the error is MSAL's interaction-in-progress error.
   */
  isInteractionInProgressError: (err: unknown) => boolean;

  /**
   * Checks whether Microsoft rejected the sign-in because this tenant is not enabled.
   *
   * This is used to route users to a friendlier access-not-enabled state instead
   * of surfacing the raw Microsoft error.
   *
   * @param err The caught error to inspect.
   * @returns `true` when the error looks like an unsupported tenant/service principal issue.
   */
  isUnsupportedTenantAccessError: (err: unknown) => boolean;
}

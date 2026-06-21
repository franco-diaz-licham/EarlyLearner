import { type Configuration, type RedirectRequest } from '@azure/msal-browser';
import { appConfig } from '../../../shared/config/appConfig';

export const msalConfig: Configuration = {
  auth: {
    clientId: appConfig.entraClientId,
    authority: appConfig.entraAuthority,
    knownAuthorities: appConfig.entraAuthority ? [new URL(appConfig.entraAuthority).host] : [],
    redirectUri: window.location.origin,
    postLogoutRedirectUri: window.location.origin
  },
  cache: {
    cacheLocation: 'sessionStorage'
  }
};

export const loginRequest: RedirectRequest = {
  scopes: [appConfig.entraApiScope].filter(Boolean),
  prompt: 'select_account'
};

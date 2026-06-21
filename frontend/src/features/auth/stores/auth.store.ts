import { create } from 'zustand';
import { authConnector } from '../services/authConnector';
import type { AuthAccount, AuthRedirectResult } from '../types/auth.types';

interface AuthStartupResult {
  redirectResult: AuthRedirectResult | null;
  routeToAccessNotEnabled: boolean;
}

interface AuthState {
  account: AuthAccount | null;
  hasInitialised: boolean;
  interactionInProgress: boolean;
  routeToAccessNotEnabled: boolean;
  isAuthenticated: boolean;
  initialiseAuth: () => Promise<AuthStartupResult>;
  syncAccount: () => void;
  login: () => Promise<void>;
  logout: () => Promise<void>;
  getToken: () => Promise<string | null>;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  account: null,
  hasInitialised: false,
  interactionInProgress: false,
  routeToAccessNotEnabled: false,
  get isAuthenticated() {
    return get().account !== null;
  },

  syncAccount: () => {
    set({ account: authConnector.getCurrentAccount() });
  },

  initialiseAuth: async () => {
    set({ interactionInProgress: true });

    let redirectResult: AuthRedirectResult | null = null;
    let routeToAccessNotEnabled = false;

    try {
      await authConnector.initialize();
      redirectResult = await authConnector.handleRedirect();
    } catch (err) {
      if (!authConnector.isUnsupportedTenantAccessError(err)) throw err;
      routeToAccessNotEnabled = true;
    } finally {
      set({
        account: authConnector.getCurrentAccount(),
        hasInitialised: true,
        interactionInProgress: false,
        routeToAccessNotEnabled
      });
    }

    return { redirectResult, routeToAccessNotEnabled };
  },

  login: async () => {
    if (get().interactionInProgress) return;

    try {
      set({ interactionInProgress: true });
      await authConnector.login();
    } catch (err) {
      if (!authConnector.isInteractionInProgressError(err)) throw err;
    } finally {
      set({ interactionInProgress: false });
    }
  },

  logout: async () => {
    if (get().interactionInProgress) return;

    try {
      set({ interactionInProgress: true });
      await authConnector.logout();
    } catch (err) {
      if (!authConnector.isInteractionInProgressError(err)) throw err;
    } finally {
      set({ interactionInProgress: false });
    }
  },

  getToken: async () => {
    if (!get().account) return null;

    return authConnector.getToken({
      allowRedirect: !get().interactionInProgress,
      beforeRedirect: () => {
        set({ interactionInProgress: true });
      }
    });
  }
}));

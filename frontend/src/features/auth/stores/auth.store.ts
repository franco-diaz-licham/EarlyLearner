import { create } from 'zustand';
import { authConnector } from '../services/authConnector';
import type { AuthAccount } from '../types/auth.types';

interface AuthState {
  account: AuthAccount | null;
  hasInitialised: boolean;
  interactionInProgress: boolean;
  routeToAccessNotEnabled: boolean;
  initialiseAuth: () => Promise<void>;
  syncAccount: () => void;
  login: (redirectStartPage?: string) => Promise<void>;
  createAccount: (redirectStartPage?: string) => Promise<void>;
  logout: () => Promise<void>;
  getAccessToken: () => Promise<string | null>;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  account: null,
  hasInitialised: false,
  interactionInProgress: false,
  routeToAccessNotEnabled: false,

  syncAccount: () => {
    set({ account: authConnector.getCurrentAccount() });
  },

  initialiseAuth: async () => {
    set({ interactionInProgress: true });

    let routeToAccessNotEnabled = false;

    try {
      await authConnector.initialize();
      await authConnector.handleRedirect();
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
  },

  login: async (redirectStartPage) => {
    if (get().interactionInProgress) return;

    try {
      set({ interactionInProgress: true });
      await authConnector.login(redirectStartPage);
    } catch (err) {
      if (!authConnector.isInteractionInProgressError(err)) throw err;
    } finally {
      set({ interactionInProgress: false });
    }
  },

  createAccount: async (redirectStartPage) => {
    if (get().interactionInProgress) return;

    try {
      set({ interactionInProgress: true });
      await authConnector.createAccount(redirectStartPage);
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

  getAccessToken: async () => {
    if (!get().account) return null;

    return authConnector.getAccessToken({
      allowRedirect: !get().interactionInProgress,
      beforeRedirect: () => {
        set({ interactionInProgress: true });
      }
    });
  }
}));

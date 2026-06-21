import { useAuthStore } from '../stores/auth.store';

export const useAuth = () =>
  useAuthStore((state) => ({
    account: state.account,
    isAuthenticated: state.isAuthenticated,
    isAuthReady: state.hasInitialised && !state.interactionInProgress,
    interactionInProgress: state.interactionInProgress,
    routeToAccessNotEnabled: state.routeToAccessNotEnabled,
    login: state.login,
    logout: state.logout,
    getToken: state.getToken
  }));

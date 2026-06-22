import { useAuthStore } from '../stores/auth.store';

export const useAuth = () => {
  const account = useAuthStore((state) => state.account);
  const hasInitialised = useAuthStore((state) => state.hasInitialised);
  const interactionInProgress = useAuthStore((state) => state.interactionInProgress);
  const routeToAccessNotEnabled = useAuthStore((state) => state.routeToAccessNotEnabled);
  const login = useAuthStore((state) => state.login);
  const createAccount = useAuthStore((state) => state.createAccount);
  const logout = useAuthStore((state) => state.logout);
  const getAccessToken = useAuthStore((state) => state.getAccessToken);

  return {
    account,
    isAuthenticated: account !== null,
    isAuthReady: hasInitialised && !interactionInProgress,
    interactionInProgress,
    routeToAccessNotEnabled,
    login,
    createAccount,
    logout,
    getAccessToken
  };
};

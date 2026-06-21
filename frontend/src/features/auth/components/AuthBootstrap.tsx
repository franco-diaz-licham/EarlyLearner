import { type PropsWithChildren, useEffect } from 'react';
import { useAuthStore } from '../stores/auth.store';

export const AuthBootstrap = ({ children }: PropsWithChildren) => {
  const hasInitialised = useAuthStore((state) => state.hasInitialised);
  const initialiseAuth = useAuthStore((state) => state.initialiseAuth);

  useEffect(() => {
    if (!hasInitialised) void initialiseAuth();
  }, [hasInitialised, initialiseAuth]);

  return <>{children}</>;
};

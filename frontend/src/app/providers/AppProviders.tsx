import type { PropsWithChildren } from 'react';
import { QueryClientProvider } from '@tanstack/react-query';
import { PrimeReactProvider } from 'primereact/api';
import { AuthBootstrap } from '../../features/auth';
import { queryClient } from '../../shared/api/queryClient';
import { AppGlobalHost } from '../../shared/components/AppGlobalHost';
import { primeReactConfig } from '../../shared/config/primeReactConfig';

export const AppProviders = ({ children }: PropsWithChildren) => (
  <PrimeReactProvider value={primeReactConfig}>
    <QueryClientProvider client={queryClient}>
      <AuthBootstrap>
        {children}
        <AppGlobalHost />
      </AuthBootstrap>
    </QueryClientProvider>
  </PrimeReactProvider>
);

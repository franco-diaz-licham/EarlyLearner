import type { PropsWithChildren } from 'react';
import { QueryClientProvider } from '@tanstack/react-query';
import { PrimeReactProvider } from 'primereact/api';
import { queryClient } from '../../shared/api/queryClient';
import { primeReactConfig } from '../../shared/config/primeReactConfig';

export const AppProviders = ({ children }: PropsWithChildren) => (
  <PrimeReactProvider value={primeReactConfig}>
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  </PrimeReactProvider>
);

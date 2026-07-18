import { RouterProvider } from 'react-router-dom';
import { router } from './router';
import { PrimeReactProvider } from 'primereact/api';
import { QueryClientProvider } from '@tanstack/react-query';
import { AuthBootstrap } from '../features/auth';
import { ConfirmProvider } from '../shared/confirm/ConfirmProvider';
import { AppToast } from '../shared/ui/AppToast';
import { primeReactConfig } from '../shared/config/primeReactConfig';
import { queryClient } from '../shared/api/queryClient';

export const App = () => (
  <PrimeReactProvider value={primeReactConfig}>
    <QueryClientProvider client={queryClient}>
      <AuthBootstrap>
        <ConfirmProvider>
          <RouterProvider router={router} />
        </ConfirmProvider>
        <AppToast />
      </AuthBootstrap>
    </QueryClientProvider>
  </PrimeReactProvider>
);

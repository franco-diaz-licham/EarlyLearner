import { RouterProvider } from 'react-router-dom';
import { AuthBootstrap } from '../features/auth';
import { AppProviders } from './providers/AppProviders';
import { router } from './router';

export const App = () => (
  <AppProviders>
    <AuthBootstrap>
      <RouterProvider router={router} />
    </AuthBootstrap>
  </AppProviders>
);

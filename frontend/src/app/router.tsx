import { createBrowserRouter, Navigate } from 'react-router-dom';
import { LoginPage, RequireAuth } from '../features/auth';
import { AppShell } from '../shared/layout/AppShell';
import { HomePage } from '../features/home/pages/HomePage';
import { HouseholdsPage } from '../features/households/pages/HouseholdsPage';
import { LearningPage } from '../features/learning/pages/LearningPage';
import { NotFoundPage } from '../shared/pages/NotFoundPage';

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  {
    element: <RequireAuth />,
    children: [
      {
        path: '/',
        element: <AppShell />,
        children: [
          { index: true, element: <Navigate to="/home" replace /> },
          { path: 'home', element: <HomePage /> },
          { path: 'households', element: <HouseholdsPage /> },
          { path: 'learning', element: <LearningPage /> },
          { path: '*', element: <NotFoundPage /> }
        ]
      }
    ]
  }
]);

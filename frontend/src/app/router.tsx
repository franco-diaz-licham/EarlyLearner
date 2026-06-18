import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppShell } from '../shared/layout/AppShell';
import { HomePage } from '../features/home/pages/HomePage';
import { NotFoundPage } from '../shared/pages/NotFoundPage';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <Navigate to="/home" replace /> },
      { path: 'home', element: <HomePage /> },
      { path: '*', element: <NotFoundPage /> }
    ]
  }
]);

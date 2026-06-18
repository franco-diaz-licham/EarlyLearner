import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppShell } from '../shared/layout/AppShell';
import { HomePage } from '../features/home/pages/HomePage';
import { LearningPage } from '../features/learning-records/pages/LearningPage';
import { PlanningPage } from '../features/planning/pages/PlanningPage';
import { ReadinessPage } from '../features/readiness/pages/ReadinessPage';
import { NotFoundPage } from '../shared/pages/NotFoundPage';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <Navigate to="/home" replace /> },
      { path: 'home', element: <HomePage /> },
      { path: 'planning', element: <PlanningPage /> },
      { path: 'readiness', element: <ReadinessPage /> },
      { path: 'learning', element: <LearningPage /> },
      { path: '*', element: <NotFoundPage /> }
    ]
  }
]);

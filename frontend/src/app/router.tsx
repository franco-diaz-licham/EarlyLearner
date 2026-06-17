import { createBrowserRouter, Navigate } from "react-router-dom";
import { AppShell } from "../shared/layout/AppShell";
import { DashboardPage } from "../features/dashboard/pages/DashboardPage";
import { NotFoundPage } from "../shared/pages/NotFoundPage";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <AppShell />,
        children: [
            { index: true, element: <Navigate to="/dashboard" replace /> },
            { path: "dashboard", element: <DashboardPage /> },
            { path: "*", element: <NotFoundPage /> },
        ],
    },
]);

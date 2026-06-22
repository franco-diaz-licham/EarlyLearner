import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export const RequireAuth = () => {
  const location = useLocation();
  const { isAuthenticated, isAuthReady, routeToAccessNotEnabled } = useAuth();

  if (routeToAccessNotEnabled) return <Navigate to="/login?access=not-enabled" replace />;

  if (!isAuthReady) {
    return (
      <div className="min-h-screen bg-brand-background">
        <main className="mx-auto w-full max-w-355 px-4 py-4 sm:px-6 lg:px-8">
          <div className="h-16 animate-pulse rounded-md border border-brand-border/70 bg-white/80 shadow-app-card" />
        </main>
      </div>
    );
  }

  if (!isAuthenticated) {
    const returnTo = `${location.pathname}${location.search}${location.hash}`;
    return <Navigate to={`/login?returnTo=${encodeURIComponent(returnTo)}`} replace />;
  }

  return <Outlet />;
};

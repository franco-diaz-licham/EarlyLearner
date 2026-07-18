import { Outlet } from 'react-router-dom';
import { useAuth } from '../../features/auth';
import { AppTopBar } from './AppTopBar';

export const AppShell = () => {
  const { logout } = useAuth();

  const handleLogout = () => {
    void logout();
  };

  return (
    <div className="h-dvh bg-brand-background">
      <main className="mx-auto flex h-full w-full max-w-355 flex-col gap-4 px-4 py-4 sm:px-6 lg:px-8">
        <AppTopBar onLogout={handleLogout} />
        <div className="min-h-0 flex-1 overflow-y-auto">
          <Outlet />
        </div>
      </main>
    </div>
  );
};

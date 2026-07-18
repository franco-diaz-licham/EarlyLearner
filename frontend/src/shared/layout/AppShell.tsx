import { Outlet } from 'react-router-dom';
import { useAuth } from '../../features/auth';
import { AppTopBar } from './AppTopBar';

export const AppShell = () => {
  const { logout } = useAuth();

  const handleLogout = () => {
    void logout();
  };

  return (
    <div className="min-h-screen bg-brand-background ">
      <main className="mx-auto w-full max-w-355 px-4 pt-4 sm:px-6 lg:px-8 space-y-4">
        <AppTopBar onLogout={handleLogout} />
        <Outlet />
      </main>
    </div>
  );
};

import { Outlet } from 'react-router-dom';
import { AppTopBar } from './AppTopBar';

export const AppShell = () => (
  <div className="min-h-screen bg-[#fbfaf7]">
    <main className="mx-auto w-full max-w-[1420px] px-4 py-4 sm:px-6 lg:px-8 space-y-4">
      <AppTopBar />
      <Outlet />
    </main>
  </div>
);

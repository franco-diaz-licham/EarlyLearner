import { Outlet } from 'react-router-dom';

export const AppShell = () => (
  <div className="min-h-screen bg-[#fbfaf7]">
    <main className="mx-auto w-full max-w-[1420px] px-4 py-4 sm:px-6 lg:px-8">
      <Outlet />
    </main>
  </div>
);

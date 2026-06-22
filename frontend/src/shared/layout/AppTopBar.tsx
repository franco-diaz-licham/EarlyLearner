import { UilBell, UilSearch } from '@iconscout/react-unicons';
import { NavLink } from 'react-router-dom';
import LightLogo from '../../assets/logo.png';
import { AppIconButton } from '../ui/AppIconButton';
import { AppAccountMenu } from './AppAccountMenu';

const navigationItems = [
  { label: 'Home', path: '/home' },
  { label: 'Planning', path: '/planning' },
  { label: 'Readiness', path: '/readiness' },
  { label: 'Learning', path: '/learning' }
] as const;

interface AppTopBarProps {
  onLogout?: () => void;
}

export const AppTopBar = ({ onLogout }: AppTopBarProps) => (
  <header className="flex min-h-16 items-center justify-between gap-4 rounded-md border border-brand-border/70 bg-white/90 px-4 shadow-app-card  sm:px-6">
    <div className="flex min-w-0 items-center gap-2">
      <img src={LightLogo} alt="logo" className="layout-topbar-logo-image" width={30} />
      <span className="text-lg font-extrabold text-brand-heading">EarlyLearner</span>
    </div>

    <nav aria-label="Primary" className="hidden flex-1 items-center justify-center gap-8 lg:flex">
      {navigationItems.map((item) => (
        <NavLink className={({ isActive }) => `relative flex min-h-12 items-center px-1 text-sm font-semibold ${isActive ? 'text-[#ef7676]' : 'text-brand-heading'}`} key={item.path} to={item.path}>
          {({ isActive }) => (
            <>
              {item.label}
              <span className={`absolute bottom-1 left-1/2 h-1 w-1 -translate-x-1/2 rounded-full ${isActive ? 'bg-[#ef7676]' : 'bg-brand-muted'}`} />
            </>
          )}
        </NavLink>
      ))}
    </nav>

    <div className="flex items-center gap-3">
      <AppIconButton aria-label="Search" icon={<UilSearch aria-hidden="true" className="h-5 w-5" />} />
      <AppIconButton aria-label="Notifications" icon={<UilBell aria-hidden="true" className="h-6 w-6" />} />
      <AppAccountMenu onLogout={onLogout} />
    </div>
  </header>
);

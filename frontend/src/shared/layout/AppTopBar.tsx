import { Link } from 'react-router-dom';
import { NotificationsButton } from '../../features/notifications/components/NotificationsButton';
import LightLogo from '../../assets/logo.png';
import { AppAccountMenu } from './AppAccountMenu';

interface AppTopBarProps {
  onLogout?: () => void;
}

export const AppTopBar = ({ onLogout }: AppTopBarProps) => (
  <header className="flex min-h-16 items-center justify-between gap-4 rounded-md border border-brand-border/70 bg-white/90 px-4 shadow-app-card  sm:px-6">
    <Link className="flex min-w-0 items-center gap-2 rounded-md focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-brand-primary" to="/home">
      <img src={LightLogo} alt="logo" className="layout-topbar-logo-image" width={30} />
      <span className="text-lg font-extrabold text-brand-heading">EarlyLearner</span>
    </Link>

    <div className="flex items-center gap-3">
      <NotificationsButton />
      <AppAccountMenu onLogout={onLogout} />
    </div>
  </header>
);

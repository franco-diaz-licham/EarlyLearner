import { UilAngleDown } from '@iconscout/react-unicons';
import type { MouseEvent } from 'react';
import { useRef } from 'react';
import { Button } from 'primereact/button';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useNavigate } from 'react-router-dom';
import { AppButton } from '../ui/AppButton';
import { useSessionStore } from '../stores/sessionStore';

interface AppAccountMenuProps {
  onLogout?: () => void;
}

export const AppAccountMenu = ({ onLogout }: AppAccountMenuProps) => {
  const overlayPanelRef = useRef<OverlayPanel>(null);
  const navigate = useNavigate();
  const currentUser = useSessionStore((state) => state.currentUser);

  const handleToggle = (event: MouseEvent<HTMLButtonElement>) => {
    overlayPanelRef.current?.toggle(event);
  };

  const handleLogout = () => {
    overlayPanelRef.current?.hide();
    onLogout?.();
  };

  const handleManageHouseholds = () => {
    overlayPanelRef.current?.hide();
    void navigate('/households');
  };

  return (
    <>
      <Button aria-haspopup="dialog" className="app-account-menu-button flex items-center gap-3 rounded-3xl px-1 py-1 hover:bg-brand-surface-muted" type="button" onClick={handleToggle}>
        <span className="h-10 w-10 rounded-full bg-[linear-gradient(135deg,#f7c7b6,#f6e4a7_45%,#b9d7c5)]" />
        <span className="hidden text-left sm:block">
          <span className="block text-sm font-bold leading-4 text-brand-heading">{currentUser.displayName}</span>
          <span className="block text-xs leading-4 text-brand-muted">{currentUser.roleLabel}</span>
        </span>
        <UilAngleDown aria-hidden="true" className="h-4 w-4" />
      </Button>

      <OverlayPanel ref={overlayPanelRef} className="rounded-md! border! border-brand-border! bg-white shadow-app-card!" dismissable>
        <div className="w-64 p-2 flex flex-col gap-2">
          <div className="border-b border-brand-border px-2 pb-4">
            <p className="text-sm font-bold text-brand-heading">{currentUser.displayName}</p>
            <p className="mt-1 text-xs font-semibold text-brand-muted">{currentUser.roleLabel}</p>
            <p className="mt-2 text-xs text-brand-muted">{currentUser.organisationName}</p>
          </div>
          <AppButton className="mt-3 w-full justify-center" variant="secondary" onClick={handleManageHouseholds}>
            Manage households
          </AppButton>
          <AppButton className="mt-3 w-full justify-center" variant="secondary" onClick={handleLogout}>
            Log out
          </AppButton>
        </div>
      </OverlayPanel>
    </>
  );
};

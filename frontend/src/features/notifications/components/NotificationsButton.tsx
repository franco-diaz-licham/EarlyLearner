import { UilBell } from '@iconscout/react-unicons';
import { useMemo, useRef } from 'react';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useHouseholdsQuery } from '../../households/queries/household.queries';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { useNotificationStream } from '../hooks/useNotificationStream';

interface ActiveInvitations {
  householdId: string;
  invitationId: string;
  invitedAt: string;
}

const activeInvitationStatuses = new Set(['invited', 'pending']);

const formatNotificationTime = (occurredAt: string) => {
  const date = new Date(occurredAt);
  if (Number.isNaN(date.getTime())) return '';
  return new Intl.DateTimeFormat(undefined, { dateStyle: 'short', timeStyle: 'short' }).format(date);
};

export const NotificationsButton = () => {
  const panelRef = useRef<OverlayPanel>(null);
  const householdsQuery = useHouseholdsQuery();

  const subscription = useMemo<ActiveInvitations | null>(() => {
    const activeInvitations = [];
    for (const household of householdsQuery.data ?? []) {
      for (const invitation of household.invitations) {
        if (activeInvitationStatuses.has(invitation.status.toLowerCase())) {
          activeInvitations.push({
            householdId: household.id,
            invitationId: invitation.id,
            invitedAt: invitation.invitedAt
          });
        }
      }
    }

    return activeInvitations.sort((first, second) => Date.parse(second.invitedAt) - Date.parse(first.invitedAt))[0] ?? null;
  }, [householdsQuery.data]);

  const { notifications, status } = useNotificationStream(subscription);
  const hasNewNotifications = notifications.length > 0;

  return (
    <div className="relative">
      <AppIconButton
        aria-label="Notifications"
        icon={
          <span className="relative">
            <UilBell aria-hidden="true" className="h-6 w-6" />
            {hasNewNotifications ? <span className="absolute right-0 top-0 h-2 w-2 rounded-full bg-[#ef7676]" /> : null}
          </span>
        }
        onClick={(event) => panelRef.current?.toggle(event)}
      />

      <OverlayPanel ref={panelRef} className="app-notifications-panel rounded-md! border! border-brand-border! bg-white shadow-app-card!" dismissable>
        <div className="flex w-80 flex-col gap-3 p-2">
          <div className="border-b border-brand-border px-2 pb-3">
            <p className="text-sm font-bold text-brand-heading">Notifications</p>
            <p className="mt-1 text-xs font-semibold text-brand-muted">{subscription ? `Stream ${status}` : 'No active invitations to watch.'}</p>
          </div>

          {notifications.length ? (
            <ul className="max-h-80 space-y-2 overflow-auto px-1">
              {notifications.map((notification) => (
                <li className="rounded-md border border-brand-border/70 bg-white p-3" key={notification.id}>
                  <p className="text-sm font-semibold text-brand-heading">{notification.title}</p>
                  <p className="mt-1 text-sm text-brand-muted">{notification.message}</p>
                  <p className="mt-2 text-xs text-brand-muted">{formatNotificationTime(notification.occurredAt)}</p>
                </li>
              ))}
            </ul>
          ) : (
            <p className="rounded-md bg-brand-surface-muted p-3 text-sm text-brand-muted">No notifications yet.</p>
          )}
        </div>
      </OverlayPanel>
    </div>
  );
};

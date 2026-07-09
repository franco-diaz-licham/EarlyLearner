import { UilBell } from '@iconscout/react-unicons';
import { useMemo, useRef } from 'react';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useHouseholdsQuery } from '../../households/queries/household.queries';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { useNotificationStream } from '../hooks/useNotificationStream';

const activeInvitationStatuses = new Set(['invited', 'pending']);

const formatNotificationTime = (occurredAt: string) => {
  const date = new Date(occurredAt);
  if (Number.isNaN(date.getTime())) return '';
  return new Intl.DateTimeFormat(undefined, { dateStyle: 'short', timeStyle: 'short' }).format(date);
};

export const NotificationsButton = () => {
  const panelRef = useRef<OverlayPanel>(null);
  const householdsQuery = useHouseholdsQuery();
  const invitationId = useMemo(() => {
    const invitations = householdsQuery.data?.flatMap((household) => household.invitations) ?? [];
    return invitations.find((invitation) => activeInvitationStatuses.has(invitation.status.toLowerCase()))?.id ?? null;
  }, [householdsQuery.data]);

  const { notifications, status } = useNotificationStream(invitationId);
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

      <OverlayPanel ref={panelRef} className="w-80">
        <div className="space-y-3">
          <div>
            <p className="text-sm font-bold text-brand-heading">Notifications</p>
            <p className="text-xs text-brand-muted">{invitationId ? `Stream ${status}` : 'No active invitations to watch.'}</p>
          </div>

          {notifications.length ? (
            <ul className="max-h-80 space-y-3 overflow-auto">
              {notifications.map((notification) => (
                <li className="rounded-md border border-brand-border/70 p-3" key={notification.id}>
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

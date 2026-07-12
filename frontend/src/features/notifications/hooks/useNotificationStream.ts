import { useCallback, useEffect, useState } from 'react';
import { notificationService } from '../services/notification.services';
import { subscribeToNotificationStream } from '../services/notificationStream.services';
import type { NotificationModel, NotificationStreamStatus } from '../types/notification.types';

const notificationLimit = 20;

const prependNotifications = (current: NotificationModel[], next: NotificationModel[]): NotificationModel[] => {
  return [...next, ...current].slice(0, notificationLimit);
};

type ActiveNotificationStreamStatus = Exclude<NotificationStreamStatus, 'idle'>;

interface StartNotificationStreamOptions {
  householdId: string;
  invitationId: string;
  signal: AbortSignal;
  onStatusChanged: (status: ActiveNotificationStreamStatus) => void;
  onNotifications: (notifications: NotificationModel[]) => void;
}

const startNotificationStream = async ({ householdId, invitationId, signal, onStatusChanged, onNotifications }: StartNotificationStreamOptions) => {
  try {
    await subscribeToNotificationStream({
      householdId,
      invitationId,
      signal,
      onConnected: () => {
        if (!signal.aborted) onStatusChanged('connected');
      },
      onNotifications
    });
  } catch {
    if (!signal.aborted) onStatusChanged('error');
  }
};

export const useNotificationStream = (subscription: { householdId: string; invitationId: string } | null) => {
  const [notifications, setNotifications] = useState<NotificationModel[]>([]);
  const [streamStatus, setStreamStatus] = useState<{ invitationId: string; status: ActiveNotificationStreamStatus } | null>(null);

  useEffect(() => {
    if (!subscription) return;

    const abortController = new AbortController();

    void startNotificationStream({
      householdId: subscription.householdId,
      invitationId: subscription.invitationId,
      signal: abortController.signal,
      onStatusChanged: (status) => {
        setStreamStatus({ invitationId: subscription.invitationId, status });
      },
      onNotifications: (nextNotifications) => {
        setNotifications((current) => prependNotifications(current, nextNotifications));
      }
    });

    return () => {
      abortController.abort();
    };
  }, [subscription]);

  const status: NotificationStreamStatus = !subscription ? 'idle' : streamStatus?.invitationId === subscription.invitationId ? streamStatus.status : 'connecting';

  const dismissNotification = useCallback(async (notification: NotificationModel) => {
    await notificationService.dismiss(notification.id, notification.householdId);
    setNotifications((current) => current.filter((item) => item.id !== notification.id));
  }, []);

  return { dismissNotification, notifications, status };
};

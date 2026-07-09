import { useEffect, useState } from 'react';
import { useAuthStore } from '../../auth/stores/auth.store';
import { subscribeToNotificationStream } from '../services/notificationStream.services';
import type { NotificationModel, NotificationStreamStatus } from '../types/notification.types';

const notificationLimit = 20;

const prependNotifications = (current: NotificationModel[], next: NotificationModel[]): NotificationModel[] => {
  return [...next, ...current].slice(0, notificationLimit);
};

type ActiveNotificationStreamStatus = Exclude<NotificationStreamStatus, 'idle'>;

interface StartNotificationStreamOptions {
  invitationId: string;
  signal: AbortSignal;
  onStatusChanged: (status: ActiveNotificationStreamStatus) => void;
  onNotifications: (notifications: NotificationModel[]) => void;
}

const startNotificationStream = async ({ invitationId, signal, onStatusChanged, onNotifications }: StartNotificationStreamOptions) => {
  try {
    const token = await useAuthStore.getState().getAccessToken();

    await subscribeToNotificationStream({
      invitationId,
      signal,
      token,
      onConnected: () => {
        if (!signal.aborted) onStatusChanged('connected');
      },
      onNotifications
    });
  } catch {
    if (!signal.aborted) onStatusChanged('error');
  }
};

export const useNotificationStream = (invitationId: string | null) => {
  const [notifications, setNotifications] = useState<NotificationModel[]>([]);
  const [streamStatus, setStreamStatus] = useState<{ invitationId: string; status: ActiveNotificationStreamStatus } | null>(null);

  useEffect(() => {
    if (!invitationId) return;

    const abortController = new AbortController();

    void startNotificationStream({
      invitationId,
      signal: abortController.signal,
      onStatusChanged: (status) => {
        setStreamStatus({ invitationId, status });
      },
      onNotifications: (nextNotifications) => {
        setNotifications((current) => prependNotifications(current, nextNotifications));
      }
    });

    return () => {
      abortController.abort();
    };
  }, [invitationId]);

  const status: NotificationStreamStatus = !invitationId ? 'idle' : streamStatus?.invitationId === invitationId ? streamStatus.status : 'connecting';

  return { notifications, status };
};

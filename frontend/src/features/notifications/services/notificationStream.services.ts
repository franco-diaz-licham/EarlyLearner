import { createHubConnection } from '../../../shared/api/hubClient';
import type { NotificationModel } from '../types/notification.types';

interface SubscribeToNotificationStreamOptions {
  householdId: string;
  invitationId: string;
  signal: AbortSignal;
  onConnected: () => void;
  onNotifications: (notifications: NotificationModel[]) => void;
}

/**
 * SignalR client method name used by the API when pushing notification payloads.
 * This must match NotificationHub.NotificationReceivedMethod on the backend.
 */
const notificationReceivedMethod = 'notification';

export const subscribeToNotificationStream = async ({ householdId, invitationId, signal, onConnected, onNotifications }: SubscribeToNotificationStreamOptions) => {
  const connection = createHubConnection('/hubs/notifications', {
    query: { householdId, invitationId }
  });

  let isStopping = false;
  const stopConnection = () => {
    isStopping = true;
    void connection.stop();
  };

  signal.addEventListener('abort', stopConnection, { once: true });

  connection.on(notificationReceivedMethod, (notification: NotificationModel) => {
    onNotifications([notification]);
  });

  try {
    await connection.start();
    if (signal.aborted) return;

    onConnected();

    await new Promise<void>((resolve, reject) => {
      connection.onclose((error) => {
        if (isStopping || signal.aborted) resolve();
        else reject(error ?? new Error('Notification connection closed.'));
      });
    });
  } finally {
    signal.removeEventListener('abort', stopConnection);
    stopConnection();
  }
};

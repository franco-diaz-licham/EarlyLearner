import { HubConnectionBuilder, HubConnectionState, HttpTransportType } from '@microsoft/signalr';
import { appConfig } from '../../../shared/config/appConfig';
import type { NotificationModel } from '../types/notification.types';

interface SubscribeToNotificationStreamOptions {
  invitationId: string;
  signal: AbortSignal;
  token: string | null;
  onConnected: () => void;
  onNotifications: (notifications: NotificationModel[]) => void;
}

export const subscribeToNotificationStream = async ({ invitationId, signal, token, onConnected, onNotifications }: SubscribeToNotificationStreamOptions) => {
  const connection = new HubConnectionBuilder()
    .withUrl(`${appConfig.apiBaseUrl}/hubs/notifications`, {
      accessTokenFactory: () => token ?? '',
      transport: HttpTransportType.ServerSentEvents
    })
    .withAutomaticReconnect()
    .build();

  let isStopping = false;
  const stopConnection = () => {
    isStopping = true;
    if (connection.state !== HubConnectionState.Disconnected) void connection.stop();
  };

  signal.addEventListener('abort', stopConnection, { once: true });
  connection.on('notification', (notification: NotificationModel) => {
    onNotifications([notification]);
  });

  connection.onreconnected(() => {
    void connection.invoke('SubscribeToInvitation', invitationId);
  });

  try {
    await connection.start();
    if (signal.aborted) return;

    await connection.invoke('SubscribeToInvitation', invitationId);
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

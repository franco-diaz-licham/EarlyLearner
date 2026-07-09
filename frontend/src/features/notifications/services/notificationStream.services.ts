import { apiStreamClient } from '../../../shared/api/apiStreamClient';
import { mapServerSentEventChunkToNotifications } from '../mappers/notification.mapper';
import type { NotificationModel } from '../types/notification.types';

interface SubscribeToNotificationStreamOptions {
  invitationId: string;
  signal: AbortSignal;
  token: string | null;
  onConnected: () => void;
  onNotifications: (notifications: NotificationModel[]) => void;
}

const readNotificationStream = async (stream: ReadableStream<Uint8Array>, signal: AbortSignal, onNotifications: (notifications: NotificationModel[]) => void) => {
  const reader = stream.getReader();
  const decoder = new TextDecoder();
  let pendingChunk = '';

  while (!signal.aborted) {
    const { done, value } = await reader.read();
    if (done) break;

    pendingChunk += decoder.decode(value, { stream: true });
    const boundary = pendingChunk.lastIndexOf('\n\n');
    if (boundary < 0) continue;

    const completedChunk = pendingChunk.slice(0, boundary + 2);
    pendingChunk = pendingChunk.slice(boundary + 2);

    const notifications = mapServerSentEventChunkToNotifications(completedChunk);
    if (notifications.length) onNotifications(notifications);
  }
};

export const subscribeToNotificationStream = async ({ invitationId, signal, token, onConnected, onNotifications }: SubscribeToNotificationStreamOptions) => {
  const stream = await apiStreamClient.getReadableStream(`/notifications/stream/${invitationId}`, {
    headers: { Accept: 'text/event-stream' },
    signal,
    token
  });

  onConnected();

  await readNotificationStream(stream, signal, onNotifications);
};

import type { NotificationModel } from '../types/notification.types';

export const mapServerSentEventChunkToNotifications = (chunk: string): NotificationModel[] => {
  return chunk
    .split('\n\n')
    .map((event) =>
      event
        .split('\n')
        .filter((line) => line.startsWith('data:'))
        .map((line) => line.slice(5).trim())
        .join('\n')
    )
    .filter(Boolean)
    .map((data) => JSON.parse(data) as NotificationModel);
};

export interface NotificationModel {
  id: string;
  householdId: string;
  type: string;
  title: string;
  message: string;
  occurredAt: string;
}

export type NotificationStreamStatus = 'idle' | 'connecting' | 'connected' | 'error';

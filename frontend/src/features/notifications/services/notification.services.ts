import { apiClient } from '../../../shared/api/apiClient';

const NOTIFICATIONS_URL = '/notifications';

export const notificationService = {
  async dismiss(notificationId: string, householdId: string): Promise<void> {
    await apiClient.delete(`${NOTIFICATIONS_URL}/${notificationId}?householdId=${encodeURIComponent(householdId)}`);
  }
};

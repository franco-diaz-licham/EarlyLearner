import { appConfig } from '../../../shared/config/appConfig';

export const authService = {
  async ensureSession(accessToken: string): Promise<void> {
    const response = await fetch(`${appConfig.apiBaseUrl}/identity/session`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${accessToken}`
      }
    });

    if (!response.ok) throw new Error(`Session could not be initialised. Status ${response.status.toLocaleString()}.`);
  }
};

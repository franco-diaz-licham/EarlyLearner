import { appConfig } from '../../../shared/config/appConfig';
import type { ApiSingleResponse } from '../../../shared/api/api.types';
import type { AuthSessionResponse } from '../types/auth.types';

export const authService = {
  async ensureSession(accessToken: string): Promise<AuthSessionResponse> {
    const response = await fetch(`${appConfig.apiBaseUrl}/identity/session`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${accessToken}`
      }
    });

    if (!response.ok) throw new Error(`Session could not be initialised. Status ${response.status.toLocaleString()}.`);

    const session = (await response.json()) as ApiSingleResponse<AuthSessionResponse>;
    return session.data;
  }
};

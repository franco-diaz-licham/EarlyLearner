import { HubConnectionBuilder, HttpTransportType, type HubConnection } from '@microsoft/signalr';
import { useAuthStore } from '../../features/auth/stores/auth.store';
import { appConfig } from '../config/appConfig';

type HubQueryParams = Record<string, string | number | boolean | null | undefined>;

interface CreateHubConnectionOptions {
  query?: HubQueryParams;
}

const buildHubUrl = (path: string, query?: HubQueryParams): string => {
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;
  const search = new URLSearchParams();

  for (const [key, value] of Object.entries(query ?? {})) {
    if (value === null || value === undefined) continue;
    search.set(key, String(value));
  }

  const queryString = search.toString();
  return `${appConfig.apiBaseUrl}${normalizedPath}${queryString ? `?${queryString}` : ''}`;
};

export const createHubConnection = (path: string, options: CreateHubConnectionOptions = {}): HubConnection => {
  return new HubConnectionBuilder()
    .withUrl(buildHubUrl(path, options.query), {
      accessTokenFactory: async () => (await useAuthStore.getState().getAccessToken()) ?? '',
      transport: HttpTransportType.ServerSentEvents
    })
    .withAutomaticReconnect()
    .build();
};

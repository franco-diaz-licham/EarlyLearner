const readEnvValue = (key: string, fallback = ''): string => {
  const env = import.meta.env as Record<string, unknown>;
  const value = env[key];
  if (typeof value !== 'string') return fallback;
  return value;
};

const rootUrl = readEnvValue('VITE_API_BASE_URL', '').replace(/\/+$/, '');
const apiVersion = readEnvValue('VITE_API_VERSION', 'v1').replace(/^\/+|\/+$/g, '');

export const appConfig = {
  apiBaseUrl: `${rootUrl}/api/${apiVersion}`,
  appName: readEnvValue('VITE_APP_NAME', 'EarlyLearner'),
  entraClientId: readEnvValue('VITE_ENTRA_CLIENT_ID'),
  entraAuthority: readEnvValue('VITE_ENTRA_AUTHORITY'),
  entraApiScope: readEnvValue('VITE_ENTRA_API_SCOPE')
} as const;

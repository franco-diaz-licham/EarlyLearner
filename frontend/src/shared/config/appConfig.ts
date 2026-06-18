const readEnvValue = (key: string, fallback = ''): string => {
  const env = import.meta.env as Record<string, unknown>;
  const value = env[key];
  if (typeof value !== 'string') return fallback;
  return value;
};

export const appConfig = {
  apiBaseUrl: readEnvValue('VITE_API_BASE_URL', '/api'),
  appName: readEnvValue('VITE_APP_NAME', 'EarlyLearner')
} as const;

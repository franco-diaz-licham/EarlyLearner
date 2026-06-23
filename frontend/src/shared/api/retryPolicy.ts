import { getErrorStatusCode } from './apiError';

const RETRYABLE_HTTP_STATUSES = new Set([408, 429, 502, 503, 504]);
const MAX_QUERY_RETRIES = 1;

export const isRetryableHttpStatus = (status?: number): boolean => {
  if (!status) return true;
  return RETRYABLE_HTTP_STATUSES.has(status);
};

export const shouldRetryQuery = (failureCount: number, error: unknown): boolean => {
  if (failureCount >= MAX_QUERY_RETRIES) return false;
  return isRetryableHttpStatus(getErrorStatusCode(error));
};

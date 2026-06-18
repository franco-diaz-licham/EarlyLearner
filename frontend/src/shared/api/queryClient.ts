import { QueryClient } from '@tanstack/react-query';
import { shouldRetryQuery } from './retryPolicy';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5,
      retry: shouldRetryQuery,
      refetchOnWindowFocus: false
    }
  }
});

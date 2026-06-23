import { MutationCache, QueryClient } from '@tanstack/react-query';
import { publishErrorFeedback } from '../feedback/feedbackEvents';
import { shouldRetryQuery } from './retryPolicy';

export const queryClient = new QueryClient({
  mutationCache: new MutationCache({
    onError: (error) => {
      publishErrorFeedback(error);
    }
  }),
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5,
      retry: shouldRetryQuery,
      refetchOnWindowFocus: false
    }
  }
});

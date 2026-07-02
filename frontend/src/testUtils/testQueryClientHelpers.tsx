import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { renderHook } from '@testing-library/react';
import type { ReactNode } from 'react';

/**
 * Creates a fresh React Query client for a single test.
 *
 * Retries are disabled so failed queries and mutations fail immediately instead
 * of waiting for React Query's production retry behavior.
 */
export const createTestQueryClient = () =>
  new QueryClient({
    defaultOptions: {
      queries: {
        retry: false
      },
      mutations: {
        retry: false
      }
    }
  });

/**
 * Renders a hook inside a fresh React Query provider.
 *
 * Use this for hooks that call React Query APIs like useQuery, useMutation, or
 * useQueryClient. The returned queryClient lets tests inspect or spy on cache
 * behavior.
 */
export const renderHookWithClient = <TResult,>(hook: () => TResult) => {
  const queryClient = createTestQueryClient();
  const wrapper = ({ children }: { children: ReactNode }) => <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;

  return {
    queryClient,
    ...renderHook(hook, { wrapper })
  };
};

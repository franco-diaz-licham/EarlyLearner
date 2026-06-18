import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { readinessOutcomeService } from "../services/readinessOutcome.services";
import type { CreateReadinessOutcomeRequest, UpdateReadinessOutcomeRequest } from "../types/readinessOutcome.types";

export const readinessOutcomeKeys = {
  all: ["readinessOutcomes"] as const,
  lists: () => [...readinessOutcomeKeys.all, "list"] as const,
  list: () => [...readinessOutcomeKeys.lists()] as const,
  details: () => [...readinessOutcomeKeys.all, "detail"] as const,
  detail: (readinessOutcomeId: string) => [...readinessOutcomeKeys.details(), readinessOutcomeId] as const,
};

export const useReadinessOutcomesQuery = () =>
  useQuery({
    queryKey: readinessOutcomeKeys.list(),
    queryFn: () => readinessOutcomeService.list(),
  });

export const useReadinessOutcomeQuery = (readinessOutcomeId: string) =>
  useQuery({
    queryKey: readinessOutcomeKeys.detail(readinessOutcomeId),
    queryFn: () => readinessOutcomeService.get(readinessOutcomeId),
    enabled: Boolean(readinessOutcomeId),
  });

export const useCreateReadinessOutcomeMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateReadinessOutcomeRequest) => readinessOutcomeService.create(request),
    onSuccess: (readinessOutcome) => {
      void queryClient.invalidateQueries({ queryKey: readinessOutcomeKeys.lists() });
      queryClient.setQueryData(readinessOutcomeKeys.detail(readinessOutcome.readinessOutcomeId), readinessOutcome);
    },
  });
};

export const useUpdateReadinessOutcomeMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ readinessOutcomeId, request }: { readinessOutcomeId: string; request: UpdateReadinessOutcomeRequest }) =>
      readinessOutcomeService.update(readinessOutcomeId, request),
    onSuccess: (readinessOutcome) => {
      void queryClient.invalidateQueries({ queryKey: readinessOutcomeKeys.lists() });
      queryClient.setQueryData(readinessOutcomeKeys.detail(readinessOutcome.readinessOutcomeId), readinessOutcome);
    },
  });
};

export const useDeleteReadinessOutcomeMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (readinessOutcomeId: string) => readinessOutcomeService.delete(readinessOutcomeId),
    onSuccess: (_data, readinessOutcomeId) => {
      void queryClient.invalidateQueries({ queryKey: readinessOutcomeKeys.lists() });
      queryClient.removeQueries({ queryKey: readinessOutcomeKeys.detail(readinessOutcomeId) });
    },
  });
};

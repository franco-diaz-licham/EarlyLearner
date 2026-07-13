import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { learningOutcomeService } from '../services/learningOutcome.services';
import type { CreateLearningOutcomeRequest, UpdateLearningOutcomeRequest } from '../types/learningOutcome.types';

export const learningOutcomeKeys = {
  all: ['learningOutcomes'] as const,
  lists: () => [...learningOutcomeKeys.all, 'list'] as const,
  list: () => [...learningOutcomeKeys.lists()] as const,
  details: () => [...learningOutcomeKeys.all, 'detail'] as const,
  detail: (learningOutcomeId: string) => [...learningOutcomeKeys.details(), learningOutcomeId] as const
};

export const useLearningOutcomesQuery = () =>
  useQuery({
    queryKey: learningOutcomeKeys.list(),
    queryFn: () => learningOutcomeService.list()
  });

export const useLearningOutcomeQuery = (learningOutcomeId: string) =>
  useQuery({
    queryKey: learningOutcomeKeys.detail(learningOutcomeId),
    queryFn: () => learningOutcomeService.get(learningOutcomeId),
    enabled: Boolean(learningOutcomeId)
  });

export const useCreateLearningOutcomeMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateLearningOutcomeRequest) => learningOutcomeService.create(request),
    onSuccess: (learningOutcome) => {
      void queryClient.invalidateQueries({ queryKey: learningOutcomeKeys.lists() });
      queryClient.setQueryData(learningOutcomeKeys.detail(learningOutcome.learningOutcomeId), learningOutcome);
    }
  });
};

export const useUpdateLearningOutcomeMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ learningOutcomeId, request }: { learningOutcomeId: string; request: UpdateLearningOutcomeRequest }) =>
      learningOutcomeService.update(learningOutcomeId, request),
    onSuccess: (learningOutcome) => {
      void queryClient.invalidateQueries({ queryKey: learningOutcomeKeys.lists() });
      queryClient.setQueryData(learningOutcomeKeys.detail(learningOutcome.learningOutcomeId), learningOutcome);
    }
  });
};

export const useDeleteLearningOutcomeMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (learningOutcomeId: string) => learningOutcomeService.delete(learningOutcomeId),
    onSuccess: (_data, learningOutcomeId) => {
      void queryClient.invalidateQueries({ queryKey: learningOutcomeKeys.lists() });
      queryClient.removeQueries({ queryKey: learningOutcomeKeys.detail(learningOutcomeId) });
    }
  });
};

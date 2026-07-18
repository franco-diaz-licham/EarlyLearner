import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { LearningOutcomeFormModel } from '../hooks/useLearningOutcomeForm';
import { learningOutcomeService } from '../services/learningOutcome.services';
import type { LearningOutcomeStatus } from '../types/learningOutcome.types';

interface UpdateLearningOutcome {
  learningOutcomeId: string;
  form: LearningOutcomeFormModel;
}

interface UpdateLearningOutcomeStatus {
  learningOutcomeId: string;
  status: LearningOutcomeStatus;
}

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
    mutationFn: (form: LearningOutcomeFormModel) =>
      learningOutcomeService.create({
        code: form.code,
        name: form.name,
        description: form.description,
        category: form.category,
        sortOrder: form.sortOrder
      }),
    onSuccess: (learningOutcome) => {
      void queryClient.invalidateQueries({ queryKey: learningOutcomeKeys.lists() });
      queryClient.setQueryData(learningOutcomeKeys.detail(learningOutcome.learningOutcomeId), learningOutcome);
    }
  });
};

export const useUpdateLearningOutcomeMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ learningOutcomeId, form }: UpdateLearningOutcome) =>
      learningOutcomeService.update(learningOutcomeId, {
        name: form.name,
        description: form.description,
        category: form.category,
        sortOrder: form.sortOrder
      }),
    onSuccess: (learningOutcome) => {
      void queryClient.invalidateQueries({ queryKey: learningOutcomeKeys.lists() });
      queryClient.setQueryData(learningOutcomeKeys.detail(learningOutcome.learningOutcomeId), learningOutcome);
    }
  });
};

export const useUpdateLearningOutcomeStatusMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ learningOutcomeId, status }: UpdateLearningOutcomeStatus) => learningOutcomeService.updateStatus(learningOutcomeId, { status }),
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
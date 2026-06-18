import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { learningPlanService } from '../services/learningPlan.services';
import type { CreateLearningPlanRequest, UpdateLearningPlanRequest } from '../types/learningPlan.types';

export const learningPlanKeys = {
  all: ['learningPlans'] as const,
  lists: () => [...learningPlanKeys.all, 'list'] as const,
  list: (householdId: string) => [...learningPlanKeys.lists(), householdId] as const,
  details: () => [...learningPlanKeys.all, 'detail'] as const,
  detail: (learningPlanId: string) => [...learningPlanKeys.details(), learningPlanId] as const
};

export const useLearningPlansQuery = (householdId: string) =>
  useQuery({
    queryKey: learningPlanKeys.list(householdId),
    queryFn: () => learningPlanService.list(householdId),
    enabled: Boolean(householdId)
  });

export const useLearningPlanQuery = (learningPlanId: string) =>
  useQuery({
    queryKey: learningPlanKeys.detail(learningPlanId),
    queryFn: () => learningPlanService.get(learningPlanId),
    enabled: Boolean(learningPlanId)
  });

export const useCreateLearningPlanMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateLearningPlanRequest) => learningPlanService.create(request),
    onSuccess: (learningPlan) => {
      void queryClient.invalidateQueries({ queryKey: learningPlanKeys.list(learningPlan.householdId) });
      queryClient.setQueryData(learningPlanKeys.detail(learningPlan.learningPlanId), learningPlan);
    }
  });
};

export const useUpdateLearningPlanMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ learningPlanId, request }: { learningPlanId: string; request: UpdateLearningPlanRequest }) => learningPlanService.update(learningPlanId, request),
    onSuccess: (learningPlan) => {
      void queryClient.invalidateQueries({ queryKey: learningPlanKeys.list(learningPlan.householdId) });
      queryClient.setQueryData(learningPlanKeys.detail(learningPlan.learningPlanId), learningPlan);
    }
  });
};

export const useDeleteLearningPlanMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (learningPlanId: string) => learningPlanService.delete(learningPlanId),
    onSuccess: (_data, learningPlanId) => {
      void queryClient.invalidateQueries({ queryKey: learningPlanKeys.lists() });
      queryClient.removeQueries({ queryKey: learningPlanKeys.detail(learningPlanId) });
    }
  });
};

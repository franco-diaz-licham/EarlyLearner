import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { goalService } from '../services/goal.services';
import type { CreateGoalRequest, UpdateGoalRequest } from '../types/goal.types';

export const goalKeys = {
  all: ['goals'] as const,
  lists: () => [...goalKeys.all, 'list'] as const,
  list: (householdId: string) => [...goalKeys.lists(), householdId] as const,
  details: () => [...goalKeys.all, 'detail'] as const,
  detail: (goalId: string) => [...goalKeys.details(), goalId] as const
};

export const useGoalsQuery = (householdId: string) =>
  useQuery({
    queryKey: goalKeys.list(householdId),
    queryFn: () => goalService.list(householdId),
    enabled: Boolean(householdId)
  });

export const useGoalQuery = (goalId: string) =>
  useQuery({
    queryKey: goalKeys.detail(goalId),
    queryFn: () => goalService.get(goalId),
    enabled: Boolean(goalId)
  });

export const useCreateGoalMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateGoalRequest) => goalService.create(request),
    onSuccess: (goal) => {
      void queryClient.invalidateQueries({ queryKey: goalKeys.list(goal.householdId) });
      queryClient.setQueryData(goalKeys.detail(goal.goalId), goal);
    }
  });
};

export const useUpdateGoalMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ goalId, request }: { goalId: string; request: UpdateGoalRequest }) => goalService.update(goalId, request),
    onSuccess: (goal) => {
      void queryClient.invalidateQueries({ queryKey: goalKeys.list(goal.householdId) });
      queryClient.setQueryData(goalKeys.detail(goal.goalId), goal);
    }
  });
};

export const useDeleteGoalMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (goalId: string) => goalService.delete(goalId),
    onSuccess: (_data, goalId) => {
      void queryClient.invalidateQueries({ queryKey: goalKeys.lists() });
      queryClient.removeQueries({ queryKey: goalKeys.detail(goalId) });
    }
  });
};

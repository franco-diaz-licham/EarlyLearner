import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { householdService } from '../services/household.services';
import type { CreateHouseholdRequest, UpdateHouseholdRequest } from '../types/household.types';

export const householdKeys = {
  all: ['households'] as const,
  lists: () => [...householdKeys.all, 'list'] as const,
  list: () => [...householdKeys.lists()] as const,
  details: () => [...householdKeys.all, 'detail'] as const,
  detail: (householdId: string) => [...householdKeys.details(), householdId] as const
};

export const useHouseholdsQuery = () =>
  useQuery({
    queryKey: householdKeys.list(),
    queryFn: () => householdService.list()
  });

export const useHouseholdQuery = (householdId: string) =>
  useQuery({
    queryKey: householdKeys.detail(householdId),
    queryFn: () => householdService.get(householdId),
    enabled: Boolean(householdId)
  });

export const useCreateHouseholdMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateHouseholdRequest) => householdService.create(request),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
    }
  });
};

export const useUpdateHouseholdMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ householdId, request }: { householdId: string; request: UpdateHouseholdRequest }) => householdService.update(householdId, request),
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.householdId), household);
    }
  });
};

export const useDeleteHouseholdMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (householdId: string) => householdService.delete(householdId),
    onSuccess: (_data, householdId) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.removeQueries({ queryKey: householdKeys.detail(householdId) });
    }
  });
};

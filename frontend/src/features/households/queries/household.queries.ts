import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { householdService } from '../services/household.services';
import type {
  AddHouseholdChildRequest,
  InviteHouseholdCarerRequest,
  UpdateHouseholdRequest
} from '../types/household.types';

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

export const useUpdateHouseholdMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ householdId, request }: { householdId: string; request: UpdateHouseholdRequest }) => householdService.update(householdId, request),
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useInviteHouseholdCarerMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ householdId, request }: { householdId: string; request: InviteHouseholdCarerRequest }) => householdService.inviteCarer(householdId, request),
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useRemoveHouseholdCarerMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ householdId, carerId }: { householdId: string; carerId: string }) => householdService.removeCarer(householdId, carerId),
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useAddHouseholdChildMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ householdId, request }: { householdId: string; request: AddHouseholdChildRequest }) => householdService.addChild(householdId, request),
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useRemoveHouseholdChildMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ householdId, childId }: { householdId: string; childId: string }) => householdService.removeChild(householdId, childId),
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { readinessProfileService } from '../services/readinessProfile.services';
import type { CreateReadinessProfileRequest } from '../types/readinessProfile.types';

export const readinessProfileKeys = {
  all: ['readinessProfiles'] as const,
  lists: () => [...readinessProfileKeys.all, 'list'] as const,
  list: (householdId: string) => [...readinessProfileKeys.lists(), householdId] as const,
  details: () => [...readinessProfileKeys.all, 'detail'] as const,
  detail: (readinessProfileId: string) => [...readinessProfileKeys.details(), readinessProfileId] as const
};

export const useReadinessProfilesQuery = (householdId: string) =>
  useQuery({
    queryKey: readinessProfileKeys.list(householdId),
    queryFn: () => readinessProfileService.list(householdId),
    enabled: Boolean(householdId)
  });

export const useReadinessProfileQuery = (readinessProfileId: string) =>
  useQuery({
    queryKey: readinessProfileKeys.detail(readinessProfileId),
    queryFn: () => readinessProfileService.get(readinessProfileId),
    enabled: Boolean(readinessProfileId)
  });

export const useCreateReadinessProfileMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateReadinessProfileRequest) => readinessProfileService.create(request),
    onSuccess: (readinessProfile) => {
      void queryClient.invalidateQueries({ queryKey: readinessProfileKeys.list(readinessProfile.householdId) });
      queryClient.setQueryData(readinessProfileKeys.detail(readinessProfile.readinessProfileId), readinessProfile);
    }
  });
};

export const useDeleteReadinessProfileMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (readinessProfileId: string) => readinessProfileService.delete(readinessProfileId),
    onSuccess: (_data, readinessProfileId) => {
      void queryClient.invalidateQueries({ queryKey: readinessProfileKeys.lists() });
      queryClient.removeQueries({ queryKey: readinessProfileKeys.detail(readinessProfileId) });
    }
  });
};

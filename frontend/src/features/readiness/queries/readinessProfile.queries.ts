import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { readinessProfileService } from '../services/readinessProfile.services';
import type { CreateReadinessProfileRequest } from '../types/readinessProfile.types';

export const readinessProfileKeys = {
  all: ['readinessProfiles'] as const,
  lists: () => [...readinessProfileKeys.all, 'list'] as const,
  list: () => [...readinessProfileKeys.lists(), 'current'] as const,
  details: () => [...readinessProfileKeys.all, 'detail'] as const,
  detail: (readinessProfileId: string) => [...readinessProfileKeys.details(), readinessProfileId] as const
};

export const useReadinessProfilesQuery = () =>
  useQuery({
    queryKey: readinessProfileKeys.list(),
    queryFn: () => readinessProfileService.list()
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
      void queryClient.invalidateQueries({ queryKey: readinessProfileKeys.lists() });
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

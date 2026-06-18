import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { storedFileService } from '../services/storedFile.services';
import type { CreateStoredFileRequest, UpdateStoredFileStatusRequest } from '../types/storedFile.types';

export const storedFileKeys = {
  all: ['storedFiles'] as const,
  lists: () => [...storedFileKeys.all, 'list'] as const,
  list: (householdId: string) => [...storedFileKeys.lists(), householdId] as const,
  details: () => [...storedFileKeys.all, 'detail'] as const,
  detail: (storedFileId: string) => [...storedFileKeys.details(), storedFileId] as const
};

export const useStoredFilesQuery = (householdId: string) =>
  useQuery({
    queryKey: storedFileKeys.list(householdId),
    queryFn: () => storedFileService.list(householdId),
    enabled: Boolean(householdId)
  });

export const useStoredFileQuery = (storedFileId: string) =>
  useQuery({
    queryKey: storedFileKeys.detail(storedFileId),
    queryFn: () => storedFileService.get(storedFileId),
    enabled: Boolean(storedFileId)
  });

export const useCreateStoredFileMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateStoredFileRequest) => storedFileService.create(request),
    onSuccess: (storedFile) => {
      void queryClient.invalidateQueries({ queryKey: storedFileKeys.list(storedFile.householdId) });
      queryClient.setQueryData(storedFileKeys.detail(storedFile.storedFileId), storedFile);
    }
  });
};

export const useUpdateStoredFileStatusMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ storedFileId, request }: { storedFileId: string; request: UpdateStoredFileStatusRequest }) => storedFileService.updateStatus(storedFileId, request),
    onSuccess: (storedFile) => {
      void queryClient.invalidateQueries({ queryKey: storedFileKeys.list(storedFile.householdId) });
      queryClient.setQueryData(storedFileKeys.detail(storedFile.storedFileId), storedFile);
    }
  });
};

export const useDeleteStoredFileMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (storedFileId: string) => storedFileService.delete(storedFileId),
    onSuccess: (_data, storedFileId) => {
      void queryClient.invalidateQueries({ queryKey: storedFileKeys.lists() });
      queryClient.removeQueries({ queryKey: storedFileKeys.detail(storedFileId) });
    }
  });
};

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { mapStoredFileResponseToModel, mapStoredFileResponsesToModels } from '../mappers/storedFile.mapper';
import { storedFileService } from '../services/storedFile.services';
import type { CreateStoredFileRequest, StoredFileStatus } from '../types/storedFile.types';

interface UpdateStoredFileStatus {
  storedFileId: string;
  status: StoredFileStatus;
}

export const storedFileKeys = {
  all: ['storedFiles'] as const,
  lists: () => [...storedFileKeys.all, 'list'] as const,
  list: () => [...storedFileKeys.lists(), 'current'] as const,
  details: () => [...storedFileKeys.all, 'detail'] as const,
  detail: (storedFileId: string) => [...storedFileKeys.details(), storedFileId] as const
};

export const useStoredFilesQuery = () =>
  useQuery({
    queryKey: storedFileKeys.list(),
    queryFn: async () => mapStoredFileResponsesToModels(await storedFileService.list())
  });

export const useStoredFileQuery = (storedFileId: string) =>
  useQuery({
    queryKey: storedFileKeys.detail(storedFileId),
    queryFn: async () => mapStoredFileResponseToModel(await storedFileService.get(storedFileId)),
    enabled: Boolean(storedFileId)
  });

export const useCreateStoredFileMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: CreateStoredFileRequest) => mapStoredFileResponseToModel(await storedFileService.create(request)),
    onSuccess: (storedFile) => {
      void queryClient.invalidateQueries({ queryKey: storedFileKeys.lists() });
      queryClient.setQueryData(storedFileKeys.detail(storedFile.storedFileId), storedFile);
    }
  });
};

export const useUpdateStoredFileStatusMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ storedFileId, status }: UpdateStoredFileStatus) => mapStoredFileResponseToModel(await storedFileService.updateStatus(storedFileId, { status })),
    onSuccess: (storedFile) => {
      void queryClient.invalidateQueries({ queryKey: storedFileKeys.lists() });
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

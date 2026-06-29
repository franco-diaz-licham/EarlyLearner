import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { StoredFileResponse, StoredFileUploadRequest } from '../../features/stored-files/types/storedFile.types';
import { apiClient } from '../api/apiClient';
import { fileUploadService } from '../services/fileUpload.service';

const STORED_FILES_URL = '/stored-files';
const storedFileKeys = {
  lists: () => ['storedFiles', 'list'] as const,
  detail: (storedFileId: string) => ['storedFiles', 'detail', storedFileId] as const
};

export const useCreateStoredFileUploadMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ file, mediaType, storageKey, uploadedAt }: StoredFileUploadRequest) =>
      fileUploadService.upload<StoredFileResponse>(STORED_FILES_URL, {
        file,
        fields: {
          mediaType,
          storageKey,
          uploadedAt: uploadedAt ?? new Date().toISOString()
        }
      }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: storedFileKeys.lists() });
    }
  });
};

export const useStoredFileContentQuery = (storedFileId: string | null | undefined) =>
  useQuery({
    queryKey: storedFileId ? [...storedFileKeys.detail(storedFileId), 'content'] : [...storedFileKeys.detail('none'), 'content'],
    queryFn: () => apiClient.getBlob(`${STORED_FILES_URL}/${storedFileId ?? ''}`),
    enabled: Boolean(storedFileId),
    staleTime: 5 * 60 * 1000
  });

export const useDeleteStoredFileUploadMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (storedFileId: string) => apiClient.delete(`${STORED_FILES_URL}/${storedFileId}`),
    onSuccess: (_data, storedFileId) => {
      void queryClient.invalidateQueries({ queryKey: storedFileKeys.lists() });
      queryClient.removeQueries({ queryKey: storedFileKeys.detail(storedFileId) });
    }
  });
};

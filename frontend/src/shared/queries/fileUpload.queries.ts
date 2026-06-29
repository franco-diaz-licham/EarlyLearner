import { useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '../api/apiClient';

const STORED_FILES_URL = '/stored-files';
const storedFileKeys = {
  lists: () => ['storedFiles', 'list'] as const,
  detail: (storedFileId: string) => ['storedFiles', 'detail', storedFileId] as const
};

export interface StoredFileUploadRequest {
  file: File;
  mediaType: number;
  storageKey?: string;
  uploadedAt?: string;
}

export interface StoredFileUploadResponse {
  storedFileId: string;
  householdId: string;
  storageKey: string;
  fileName: string;
  contentType: string;
  sizeInBytes: number;
  mediaType: number;
  status: number;
  uploadedAt: string;
}

const createStorageKey = (file: File) => {
  const extension = file.name.includes('.') ? file.name.split('.').pop() : undefined;
  const suffix = extension ? `.${extension}` : '';
  return `uploads/${crypto.randomUUID()}${suffix}`;
};

export const useCreateStoredFileUploadMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ file, mediaType, storageKey, uploadedAt }: StoredFileUploadRequest) =>
      apiClient.post<StoredFileUploadResponse>(STORED_FILES_URL, {
        storageKey: storageKey ?? createStorageKey(file),
        fileName: file.name,
        contentType: file.type || 'application/octet-stream',
        sizeInBytes: file.size,
        mediaType,
        uploadedAt: uploadedAt ?? new Date().toISOString()
      }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: storedFileKeys.lists() });
    }
  });
};

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

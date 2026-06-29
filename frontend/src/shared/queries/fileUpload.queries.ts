import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { StoredFileMediaType, StoredFileStatus } from '../../features/stored-files/types/storedFile.types';
import { apiClient } from '../api/apiClient';

const STORED_FILES_URL = '/stored-files';
const storedFileKeys = {
  lists: () => ['storedFiles', 'list'] as const,
  detail: (storedFileId: string) => ['storedFiles', 'detail', storedFileId] as const
};

export interface StoredFileUploadRequest {
  file: File;
  mediaType: StoredFileMediaType;
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
  mediaType: StoredFileMediaType;
  status: StoredFileStatus;
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

export const useStoredFileContentQuery = (storedFileId: string | null | undefined) =>
  useQuery({
    queryKey: storedFileId ? [...storedFileKeys.detail(storedFileId), 'content'] : [...storedFileKeys.detail('none'), 'content'],
    queryFn: () => apiClient.getBlob(`${STORED_FILES_URL}/${storedFileId ?? ''}/content`),
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

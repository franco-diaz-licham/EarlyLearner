import { apiClient } from '../../../shared/api/apiClient';
import type { CreateStoredFileRequest, StoredFileResponse, UpdateStoredFileStatusRequest } from '../types/storedFile.types';

const STORED_FILES_URL = '/stored-files';

export const storedFileService = {
  list(): Promise<StoredFileResponse[]> {
    return apiClient.getList<StoredFileResponse>(`${STORED_FILES_URL}/`);
  },

  get(storedFileId: string): Promise<StoredFileResponse> {
    return apiClient.getSingle<StoredFileResponse>(`${STORED_FILES_URL}/${storedFileId}`);
  },

  create(request: CreateStoredFileRequest): Promise<StoredFileResponse> {
    return apiClient.post<StoredFileResponse>(`${STORED_FILES_URL}/`, request);
  },

  updateStatus(storedFileId: string, request: UpdateStoredFileStatusRequest): Promise<StoredFileResponse> {
    return apiClient.put<StoredFileResponse>(`${STORED_FILES_URL}/${storedFileId}/status`, request);
  },

  delete(storedFileId: string): Promise<void> {
    return apiClient.delete(`${STORED_FILES_URL}/${storedFileId}`);
  }
};
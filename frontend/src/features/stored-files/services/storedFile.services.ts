import { apiClient } from '../../../shared/api/apiClient';
import { mapStoredFileResponseToModel, mapStoredFileResponsesToModels } from '../mappers/storedFile.mapper';
import type { CreateStoredFileRequest, StoredFileModel, StoredFileResponse, UpdateStoredFileStatusRequest } from '../types/storedFile.types';

const STORED_FILES_URL = '/stored-files';

export const storedFileService = {
  async list(householdId: string): Promise<StoredFileModel[]> {
    const storedFiles = await apiClient.getList<StoredFileResponse>(`${STORED_FILES_URL}/`, { householdId });
    return mapStoredFileResponsesToModels(storedFiles);
  },

  async get(storedFileId: string): Promise<StoredFileModel> {
    const storedFile = await apiClient.getSingle<StoredFileResponse>(`${STORED_FILES_URL}/${storedFileId}`);
    return mapStoredFileResponseToModel(storedFile);
  },

  async create(request: CreateStoredFileRequest): Promise<StoredFileModel> {
    const storedFile = await apiClient.post<StoredFileResponse>(`${STORED_FILES_URL}/`, request);
    return mapStoredFileResponseToModel(storedFile);
  },

  async updateStatus(storedFileId: string, request: UpdateStoredFileStatusRequest): Promise<StoredFileModel> {
    const storedFile = await apiClient.put<StoredFileResponse>(`${STORED_FILES_URL}/${storedFileId}/status`, request);
    return mapStoredFileResponseToModel(storedFile);
  },

  delete(storedFileId: string): Promise<void> {
    return apiClient.delete(`${STORED_FILES_URL}/${storedFileId}`);
  }
};

import type { StoredFileModel, StoredFileResponse } from '../types/storedFile.types';

export const mapStoredFileResponseToModel = (response: StoredFileResponse): StoredFileModel => ({
  storedFileId: response.storedFileId,
  householdId: response.householdId,
  storageKey: response.storageKey,
  fileName: response.fileName,
  contentType: response.contentType,
  sizeInBytes: response.sizeInBytes,
  mediaType: response.mediaType,
  status: response.status,
  uploadedAt: response.uploadedAt
});

export const mapStoredFileResponsesToModels = (responses: StoredFileResponse[]): StoredFileModel[] => responses.map(mapStoredFileResponseToModel);

export const StoredFileMediaType = {
  Photo: 'photo',
  Video: 'video',
  Document: 'document',
  Artwork: 'artwork'
} as const;

export type StoredFileMediaType = (typeof StoredFileMediaType)[keyof typeof StoredFileMediaType];

export const StoredFileStatus = {
  Pending: 'pending',
  Available: 'available',
  Rejected: 'rejected',
  Deleted: 'deleted'
} as const;

export type StoredFileStatus = (typeof StoredFileStatus)[keyof typeof StoredFileStatus];

export interface StoredFileResponse {
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

export interface StoredFileModel {
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

export interface StoredFileUploadRequest {
  file: File;
  mediaType: StoredFileMediaType;
  storageKey?: string;
  uploadedAt?: string;
}

export interface CreateStoredFileRequest {
  storageKey: string;
  fileName: string;
  contentType: string;
  sizeInBytes: number;
  mediaType: StoredFileMediaType;
  uploadedAt: string;
}

export interface UpdateStoredFileStatusRequest {
  status: StoredFileStatus;
}

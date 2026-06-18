export const StoredFileMediaType = {
  Photo: 1,
  Video: 2,
  Document: 3,
  Artwork: 4
} as const;

export type StoredFileMediaType = (typeof StoredFileMediaType)[keyof typeof StoredFileMediaType];

export const StoredFileStatus = {
  Pending: 1,
  Available: 2,
  Rejected: 3,
  Deleted: 4
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

export interface CreateStoredFileRequest {
  householdId: string;
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

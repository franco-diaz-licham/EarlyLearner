import { apiClient } from '../api/apiClient';

export interface FileUploadRequest {
  file: File;
  fieldName?: string;
  fields?: Record<string, string | number | boolean | null | undefined>;
  onProgress?: (progress: number) => void;
}

const appendFields = (form: FormData, fields?: FileUploadRequest['fields']) => {
  if (!fields) return;

  for (const [key, value] of Object.entries(fields)) {
    if (value === null || value === undefined) continue;
    form.append(key, String(value));
  }
};

export const fileUploadService = {
  upload<TResponse>(url: string, request: FileUploadRequest): Promise<TResponse> {
    const form = new FormData();
    form.append(request.fieldName ?? 'file', request.file);
    appendFields(form, request.fields);

    return apiClient.postForm<TResponse>(url, form, request.onProgress);
  }
};

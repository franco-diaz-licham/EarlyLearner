import type { ApiPaginatedResponse, ApiQueryParams, ApiSingleResponse, PaginatedResult } from './api.types';
import http from './http';

export const apiClient = {
  async getList<T>(url: string, params?: ApiQueryParams): Promise<T[]> {
    const { data } = await http.get<ApiSingleResponse<T[]>>(url, { params: params });
    return data.data;
  },

  async getPaginatedList<T>(url: string, params?: ApiQueryParams): Promise<PaginatedResult<T>> {
    const response = await http.get<ApiPaginatedResponse<T>>(url, { params });
    return {
      items: response.data.data.items,
      pagination: response.data.data.pagination
    };
  },

  async getSingle<T>(url: string, params?: ApiQueryParams): Promise<T> {
    const { data } = await http.get<ApiSingleResponse<T>>(url, { params });
    return data.data;
  },

  async post<T>(url: string, body: unknown): Promise<T> {
    const { data } = await http.post<ApiSingleResponse<T>>(url, body);
    return data.data;
  },

  async put<T>(url: string, body: unknown): Promise<T> {
    const { data } = await http.put<ApiSingleResponse<T>>(url, body);
    return data.data;
  },

  async patch<T>(url: string, body: unknown): Promise<T> {
    const { data } = await http.patch<ApiSingleResponse<T>>(url, body);
    return data.data;
  },

  async delete(url: string, body?: unknown): Promise<void> {
    if (body !== undefined) await http.delete(url, { data: body });
    else await http.delete(url);
  },

  async deleteResult<T>(url: string): Promise<T> {
    const { data } = await http.delete<ApiSingleResponse<T>>(url);
    return data.data;
  },

  async postForm<T>(url: string, form: FormData, onUploadProgress?: (progress: number) => void): Promise<T> {
    const { data } = await http.post<ApiSingleResponse<T>>(url, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
      onUploadProgress: (event) => {
        if (!onUploadProgress || !event.total) return;
        onUploadProgress(Math.round((event.loaded * 100) / event.total));
      }
    });
    return data.data;
  },

  async getBlob(url: string, params?: ApiQueryParams): Promise<Blob> {
    const resp = await http.get(url, { params, responseType: 'blob' });
    return resp.data as Blob;
  },

  async postBlob(url: string, body: unknown): Promise<Blob> {
    const resp = await http.post(url, body, { responseType: 'blob' });
    return resp.data as Blob;
  }
};

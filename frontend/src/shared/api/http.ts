import axios, { type AxiosInstance } from 'axios';
import { useAuthStore } from '../../features/auth/stores/auth.store';
import { appConfig } from '../config/appConfig';
import type { QueryFilter } from './api.types';

// Custom params serializer: encode `filters` (QueryFilter[]) as repeated
// `filters=Field:Operator:Value` entries which the backend expects.
const serializeParams = (params: Record<string, unknown>): string => {
  const search = new URLSearchParams();

  for (const [key, value] of Object.entries(params)) {
    if (value === null || value === undefined) continue;

    // QueryFilter example: { filters: [ { field: 'age', operator: 'greaterThan', value: 5 } ], pageNumber: 1 }
    if (key === 'filters' && Array.isArray(value)) {
      for (const f of value as QueryFilter[]) search.append('filters', `${f.field}:${f.operator}:${String(f.value)}`);
      continue;
    }

    // Arrays: // params input { ids: [1, 2, 3], status: 'active' }
    if (Array.isArray(value)) {
      for (const v of value) search.append(key, String(v));
      continue;
    }

    // Booleans and numbers -> toString
    if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
      search.append(key, String(value));
      continue;
    }
  }

  return search.toString();
};

const http: AxiosInstance = axios.create({
  baseURL: appConfig.apiBaseUrl,
  timeout: 30_000,
  headers: { 'Content-Type': 'application/json' },
  paramsSerializer: { serialize: serializeParams }
});

http.interceptors.request.use(async config => {
  const token = await useAuthStore.getState().getToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default http;

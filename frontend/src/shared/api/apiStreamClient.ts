import { appConfig } from '../config/appConfig';

interface ApiStreamRequestOptions {
  headers?: HeadersInit;
  signal?: AbortSignal;
  token?: string | null;
}

const buildHeaders = (options: ApiStreamRequestOptions): Headers => {
  const headers = new Headers(options.headers);
  if (options.token) headers.set('Authorization', `Bearer ${options.token}`);
  return headers;
};

const fetchApiStream = async (url: string, options: ApiStreamRequestOptions = {}): Promise<Response> => {
  return fetch(new URL(`${appConfig.apiBaseUrl}${url}`), {
    headers: buildHeaders(options),
    signal: options.signal
  });
};

const getReadableStream = async (url: string, options: ApiStreamRequestOptions = {}): Promise<ReadableStream<Uint8Array>> => {
  const response = await fetchApiStream(url, options);
  if (!response.ok || !response.body) throw new Error(`API stream failed with ${response.status.toLocaleString()}.`);
  return response.body;
};

export const apiStreamClient = {
  getReadableStream
};

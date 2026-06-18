const rootUrl = import.meta.env.VITE_API_BASE_URL as string | null;
const version = import.meta.env.VITE_API_VERSION as string | null;
const apiRoot = rootUrl ? rootUrl.replace(/\/+$/, '') : ' ';
const apiVersion = version ? version.replace(/^\/+|\/+$/g, '') : 'v1';

export const API_BASE_URL = `${apiRoot}/api/${apiVersion}`;

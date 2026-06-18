const apiRoot = import.meta.env.VITE_API_BASE_URL.replace(/\/+$/, '');
const apiVersion = (import.meta.env.VITE_API_VERSION || 'v1').replace(/^\/+|\/+$/g, '');

export const API_BASE_URL = `${apiRoot}/api/${apiVersion}`;

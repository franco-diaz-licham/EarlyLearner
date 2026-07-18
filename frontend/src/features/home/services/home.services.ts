import { apiClient } from '../../../shared/api/apiClient';
import type { GetHomeParams, HomeResponse } from '../types/home.types';

const HOME_URL = '/dashboard';

export const homeService = {
  getHome(params: GetHomeParams): Promise<HomeResponse> {
    return apiClient.getSingle<HomeResponse>(`${HOME_URL}/home`, { ...params });
  }
};
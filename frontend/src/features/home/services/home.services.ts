import { apiClient } from '../../../shared/api/apiClient';
import { mapHomeResponseToModel } from '../mappers/home.mapper';
import type { GetHomeParams, HomeModel, HomeResponse } from '../types/home.types';

const HOME_URL = '/dashboard';

export const homeService = {
  async getHome(params: GetHomeParams): Promise<HomeModel> {
    const home = await apiClient.getSingle<HomeResponse>(`${HOME_URL}/home`, { ...params });
    return mapHomeResponseToModel(home);
  }
};

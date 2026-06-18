import { apiClient } from '../../../shared/api/apiClient';
import { mapHomeDashboardResponseToModel } from '../mappers/dashboard.mapper';
import type { GetHomeDashboardParams, HomeDashboardModel, HomeDashboardResponse } from '../types/dashboard.types';

const DASHBOARD_URL = '/dashboard';

export const dashboardService = {
  async getHome(params: GetHomeDashboardParams): Promise<HomeDashboardModel> {
    const dashboard = await apiClient.getSingle<HomeDashboardResponse>(`${DASHBOARD_URL}/home`, { ...params });
    return mapHomeDashboardResponseToModel(dashboard);
  }
};

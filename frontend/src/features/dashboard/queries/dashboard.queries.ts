import { useQuery } from '@tanstack/react-query';
import { dashboardService } from '../services/dashboard.services';
import type { GetHomeDashboardParams } from '../types/dashboard.types';

export const dashboardKeys = {
  all: ['dashboard'] as const,
  home: (params: GetHomeDashboardParams) => [...dashboardKeys.all, 'home', params.householdId, params.today ?? null] as const
};

export const useHomeDashboardQuery = (params: GetHomeDashboardParams) =>
  useQuery({
    queryKey: dashboardKeys.home(params),
    queryFn: () => dashboardService.getHome(params),
    enabled: Boolean(params.householdId)
  });

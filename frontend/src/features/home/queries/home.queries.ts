import { useQuery } from '@tanstack/react-query';
import { homeService } from '../services/home.services';
import type { GetHomeParams } from '../types/home.types';

export const homeKeys = {
  all: ['home'] as const,
  current: (params: GetHomeParams) => [...homeKeys.all, params.householdId, params.today ?? null] as const
};

export const useHomeQuery = (params: GetHomeParams) =>
  useQuery({
    queryKey: homeKeys.current(params),
    queryFn: () => homeService.getHome(params),
    enabled: Boolean(params.householdId)
  });

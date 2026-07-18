import { useQuery } from '@tanstack/react-query';
import { mapHomeResponseToModel } from '../mappers/home.mapper';
import { homeService } from '../services/home.services';
import type { GetHomeParams } from '../types/home.types';

export const homeKeys = {
  all: ['home'] as const,
  current: (params: GetHomeParams) => [...homeKeys.all, params.today ?? null] as const
};

export const useHomeQuery = (params: GetHomeParams) =>
  useQuery({
    queryKey: homeKeys.current(params),
    queryFn: async () => mapHomeResponseToModel(await homeService.getHome(params))
  });
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { dailyLogService } from '../services/dailyLog.services';
import type { CreateDailyLogRequest } from '../types/dailyLog.types';

export const dailyLogKeys = {
  all: ['dailyLogs'] as const,
  lists: () => [...dailyLogKeys.all, 'list'] as const,
  list: () => [...dailyLogKeys.lists(), 'current'] as const,
  details: () => [...dailyLogKeys.all, 'detail'] as const,
  detail: (dailyLogId: string) => [...dailyLogKeys.details(), dailyLogId] as const
};

export const useDailyLogsQuery = () =>
  useQuery({
    queryKey: dailyLogKeys.list(),
    queryFn: () => dailyLogService.list()
  });

export const useDailyLogQuery = (dailyLogId: string) =>
  useQuery({
    queryKey: dailyLogKeys.detail(dailyLogId),
    queryFn: () => dailyLogService.get(dailyLogId),
    enabled: Boolean(dailyLogId)
  });

export const useCreateDailyLogMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateDailyLogRequest) => dailyLogService.create(request),
    onSuccess: (dailyLog) => {
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.lists() });
      queryClient.setQueryData(dailyLogKeys.detail(dailyLog.dailyLogId), dailyLog);
    }
  });
};

export const useDeleteDailyLogMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (dailyLogId: string) => dailyLogService.delete(dailyLogId),
    onSuccess: (_data, dailyLogId) => {
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.lists() });
      queryClient.removeQueries({ queryKey: dailyLogKeys.detail(dailyLogId) });
    }
  });
};

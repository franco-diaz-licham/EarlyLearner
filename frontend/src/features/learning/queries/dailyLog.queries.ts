import { useInfiniteQuery, useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { dailyLogService } from '../services/dailyLog.services';
import type { CreateDailyLogRequest, LearningLogFormModel } from '../types/dailyLog.types';

interface UpdateLearningMoment {
  dailyLogId: string;
  learningMomentId: string;
  form: LearningLogFormModel;
}

interface DeleteLearningMoment {
  dailyLogId: string;
  learningMomentId: string;
}

interface LearningMomentFeedQuery {
  pageSize?: number;
  searchBy?: string | null;
  searchTerm?: string | null;
}

export const dailyLogKeys = {
  all: ['dailyLogs'] as const,
  lists: () => [...dailyLogKeys.all, 'list'] as const,
  list: () => [...dailyLogKeys.lists(), 'current'] as const,
  momentFeeds: () => [...dailyLogKeys.all, 'learningMoments'] as const,
  momentFeed: (params: LearningMomentFeedQuery) => [...dailyLogKeys.momentFeeds(), params] as const,
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

export const useLearningMomentFeedQuery = ({ pageSize = 10, searchBy = null, searchTerm = null }: LearningMomentFeedQuery = {}) =>
  useInfiniteQuery({
    queryKey: dailyLogKeys.momentFeed({ pageSize, searchBy, searchTerm }),
    initialPageParam: 1,
    queryFn: ({ pageParam }) =>
      dailyLogService.listLearningMoments({
        pageNumber: pageParam,
        pageSize,
        searchBy,
        searchTerm
      }),
    getNextPageParam: (lastPage) => {
      const { pageNumber, totalPages } = lastPage.pagination;
      return pageNumber < totalPages ? pageNumber + 1 : undefined;
    }
  });

export const useCreateDailyLogMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateDailyLogRequest) => dailyLogService.create(request),
    onSuccess: (dailyLog) => {
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.lists() });
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.momentFeeds() });
      queryClient.setQueryData(dailyLogKeys.detail(dailyLog.dailyLogId), dailyLog);
    }
  });
};

export const useUpdateLearningMomentMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ dailyLogId, learningMomentId, form }: UpdateLearningMoment) =>
      dailyLogService.updateLearningMoment(dailyLogId, learningMomentId, {
        kind: form.kind,
        title: form.title,
        notes: form.notes,
        learningOutcomeIds: form.learningOutcomeIds
      }),
    onSuccess: (dailyLog) => {
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.lists() });
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.momentFeeds() });
      queryClient.setQueryData(dailyLogKeys.detail(dailyLog.dailyLogId), dailyLog);
    }
  });
};

export const useDeleteLearningMomentMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ dailyLogId, learningMomentId }: DeleteLearningMoment) => dailyLogService.deleteLearningMoment(dailyLogId, learningMomentId),
    onSuccess: (_data, { dailyLogId }) => {
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.lists() });
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.momentFeeds() });
      void queryClient.invalidateQueries({ queryKey: dailyLogKeys.detail(dailyLogId) });
    }
  });
};

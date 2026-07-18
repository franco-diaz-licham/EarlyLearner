import { act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import type { PaginatedResult } from '../../../shared/api/api.types';
import { renderHookWithClient } from '../../../testUtils/testQueryClientHelpers';
import { dailyLogService } from '../services/dailyLog.services';
import type { DailyLogModel, LearningLogFormModel, LearningMomentFeedModel } from '../types/dailyLog.types';
import { dailyLogKeys, useCreateDailyLogMutation, useDailyLogQuery, useDailyLogsQuery, useDeleteLearningMomentMutation, useLearningMomentFeedQuery, useUpdateLearningMomentMutation } from './dailyLog.queries';

vi.mock('../services/dailyLog.services', () => ({
  dailyLogService: {
    list: vi.fn(),
    get: vi.fn(),
    listLearningMoments: vi.fn(),
    create: vi.fn(),
    updateLearningMoment: vi.fn(),
    deleteLearningMoment: vi.fn()
  }
}));

interface DailyLogServiceMock {
  list: Mock;
  get: Mock;
  listLearningMoments: Mock;
  create: Mock;
  updateLearningMoment: Mock;
  deleteLearningMoment: Mock;
}

const dailyLogServiceMock = dailyLogService as unknown as DailyLogServiceMock;

const dailyLog: DailyLogModel = {
  dailyLogId: 'daily-log-1',
  householdId: 'household-1',
  childId: 'child-1',
  logDate: '2026-07-16',
  learningMomentCount: 1,
  learningMoments: [
    {
      learningMomentId: 'moment-1',
      kind: 'reading',
      title: 'Read a story',
      notes: 'Read a picture book and named familiar animals.',
      learningOutcomeIds: ['outcome-1']
    }
  ]
};

const learningMoment: LearningMomentFeedModel = {
  learningMomentId: 'moment-1',
  dailyLogId: 'daily-log-1',
  householdId: 'household-1',
  childId: 'child-1',
  logDate: '2026-07-16',
  kind: 'reading',
  title: 'Read a story',
  notes: 'Read a picture book and named familiar animals.',
  learningOutcomeIds: ['outcome-1']
};

const learningMomentPage: PaginatedResult<LearningMomentFeedModel> = {
  items: [learningMoment],
  pagination: {
    pageNumber: 1,
    totalPages: 1,
    pageSize: 10,
    totalCount: 1
  }
};

describe('daily log queries', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('loads the daily log list', async () => {
    // Arrange
    dailyLogServiceMock.list.mockResolvedValue([dailyLog]);

    // Act
    const { result } = renderHookWithClient(() => useDailyLogsQuery());

    // Assert
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    expect(dailyLogServiceMock.list).toHaveBeenCalledTimes(1);
    expect(result.current.data).toEqual([dailyLog]);
  });

  test('loads a daily log detail', async () => {
    // Arrange
    dailyLogServiceMock.get.mockResolvedValue(dailyLog);

    // Act
    const { result } = renderHookWithClient(() => useDailyLogQuery('daily-log-1'));

    // Assert
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    expect(dailyLogServiceMock.get).toHaveBeenCalledTimes(1);
    expect(dailyLogServiceMock.get).toHaveBeenCalledWith('daily-log-1');
    expect(result.current.data).toEqual(dailyLog);
  });

  test('does not load a daily log detail without an id', () => {
    // Act
    const { result } = renderHookWithClient(() => useDailyLogQuery(''));

    // Assert
    expect(result.current.fetchStatus).toBe('idle');
    expect(dailyLogServiceMock.get).not.toHaveBeenCalled();
  });

  test('loads the learning moment feed', async () => {
    // Arrange
    dailyLogServiceMock.listLearningMoments.mockResolvedValue(learningMomentPage);

    // Act
    const { result } = renderHookWithClient(() => useLearningMomentFeedQuery({ pageSize: 10, searchTerm: 'story' }));

    // Assert
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    expect(dailyLogServiceMock.listLearningMoments).toHaveBeenCalledWith({
      pageNumber: 1,
      pageSize: 10,
      searchBy: null,
      searchTerm: 'story'
    });
    expect(result.current.data?.pages).toEqual([learningMomentPage]);
  });

  test('creates a daily log and refreshes daily log query data', async () => {
    // Arrange
    const form: LearningLogFormModel = {
      childId: 'child-1',
      logDate: '2026-07-16',
      kind: 'activity',
      title: 'Paint mixing',
      notes: 'Mixed colours and described the changes.',
      learningOutcomeIds: ['outcome-1']
    };

    dailyLogServiceMock.create.mockResolvedValue(dailyLog);

    const { queryClient, result } = renderHookWithClient(() => useCreateDailyLogMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync(form);
    });

    // Assert
    expect(dailyLogServiceMock.create).toHaveBeenCalledWith({
      childId: form.childId,
      logDate: form.logDate,
      kind: form.kind,
      title: form.title,
      notes: form.notes,
      learningOutcomeIds: form.learningOutcomeIds
    });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: dailyLogKeys.lists() });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: dailyLogKeys.momentFeeds() });
    expect(queryClient.getQueryData(dailyLogKeys.detail('daily-log-1'))).toEqual(dailyLog);
  });

  test('updates a learning moment and refreshes daily log query data', async () => {
    // Arrange
    const form: LearningLogFormModel = {
      childId: 'child-1',
      logDate: '2026-07-16',
      kind: 'reading',
      title: 'Updated story',
      notes: 'Updated notes.',
      learningOutcomeIds: ['outcome-1']
    };

    dailyLogServiceMock.updateLearningMoment.mockResolvedValue(dailyLog);

    const { queryClient, result } = renderHookWithClient(() => useUpdateLearningMomentMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync({ dailyLogId: 'daily-log-1', learningMomentId: 'moment-1', form });
    });

    // Assert
    expect(dailyLogServiceMock.updateLearningMoment).toHaveBeenCalledWith('daily-log-1', 'moment-1', {
      kind: form.kind,
      title: form.title,
      notes: form.notes,
      learningOutcomeIds: form.learningOutcomeIds
    });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: dailyLogKeys.lists() });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: dailyLogKeys.momentFeeds() });
    expect(queryClient.getQueryData(dailyLogKeys.detail('daily-log-1'))).toEqual(dailyLog);
  });

  test('deletes a learning moment and refreshes daily log query data', async () => {
    // Arrange
    dailyLogServiceMock.deleteLearningMoment.mockResolvedValue(undefined);

    const { queryClient, result } = renderHookWithClient(() => useDeleteLearningMomentMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync({ dailyLogId: 'daily-log-1', learningMomentId: 'moment-1' });
    });

    // Assert
    expect(dailyLogServiceMock.deleteLearningMoment).toHaveBeenCalledWith('daily-log-1', 'moment-1');
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: dailyLogKeys.lists() });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: dailyLogKeys.momentFeeds() });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: dailyLogKeys.detail('daily-log-1') });
  });
});

import type { Mock } from 'vitest';
import { apiClient } from '../../../shared/api/apiClient';
import type { PaginatedResult } from '../../../shared/api/api.types';
import type { CreateDailyLogRequest, DailyLogResponse, LearningMomentFeedResponse } from '../types/dailyLog.types';
import { dailyLogService } from './dailyLog.services';

vi.mock('../../../shared/api/apiClient', () => ({
  apiClient: {
    getList: vi.fn(),
    getSingle: vi.fn(),
    getPaginatedList: vi.fn(),
    post: vi.fn(),
    delete: vi.fn()
  }
}));

const apiClientMock = apiClient as unknown as {
  getList: Mock;
  getSingle: Mock;
  getPaginatedList: Mock;
  post: Mock;
  delete: Mock;
};

const dailyLogResponse: DailyLogResponse = {
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

const learningMomentFeedResponse: LearningMomentFeedResponse = {
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

describe('dailyLogService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('lists daily logs', async () => {
    // Arrange
    apiClientMock.getList.mockResolvedValue([dailyLogResponse]);

    // Act
    const result = await dailyLogService.list();

    // Assert
    expect(result).toEqual([dailyLogResponse]);
    expect(apiClientMock.getList).toHaveBeenCalledTimes(1);
    expect(apiClientMock.getList).toHaveBeenCalledWith('/daily-logs/');
  });

  test('gets a daily log', async () => {
    // Arrange
    apiClientMock.getSingle.mockResolvedValue(dailyLogResponse);

    // Act
    const result = await dailyLogService.get('daily-log-1');

    // Assert
    expect(result).toEqual(dailyLogResponse);
    expect(apiClientMock.getSingle).toHaveBeenCalledTimes(1);
    expect(apiClientMock.getSingle).toHaveBeenCalledWith('/daily-logs/daily-log-1');
  });

  test('lists learning moments with pagination', async () => {
    // Arrange
    const paginatedResponse: PaginatedResult<LearningMomentFeedResponse> = {
      items: [learningMomentFeedResponse],
      pagination: {
        pageNumber: 1,
        totalPages: 1,
        pageSize: 20,
        totalCount: 1
      }
    };

    const params = { page: 1, pageSize: 20, searchTerm: 'story' };
    apiClientMock.getPaginatedList.mockResolvedValue(paginatedResponse);

    // Act
    const result = await dailyLogService.listLearningMoments(params);

    // Assert
    expect(result).toEqual(paginatedResponse);
    expect(apiClientMock.getPaginatedList).toHaveBeenCalledTimes(1);
    expect(apiClientMock.getPaginatedList).toHaveBeenCalledWith('/daily-logs/learning-moments', params);
  });

  test('creates a daily log', async () => {
    // Arrange
    const request: CreateDailyLogRequest = {
      childId: 'child-1',
      logDate: '2026-07-16',
      kind: 'activity',
      title: 'Paint mixing',
      notes: 'Mixed colours and described the changes.',
      learningOutcomeIds: ['outcome-1']
    };

    apiClientMock.post.mockResolvedValue(dailyLogResponse);

    // Act
    const result = await dailyLogService.create(request);

    // Assert
    expect(result).toEqual(dailyLogResponse);
    expect(apiClientMock.post).toHaveBeenCalledTimes(1);
    expect(apiClientMock.post).toHaveBeenCalledWith('/daily-logs/', request);
  });

  test('deletes a learning moment', async () => {
    // Arrange
    apiClientMock.delete.mockResolvedValue(undefined);

    // Act
    await dailyLogService.deleteLearningMoment('daily-log-1', 'moment-1');

    // Assert
    expect(apiClientMock.delete).toHaveBeenCalledTimes(1);
    expect(apiClientMock.delete).toHaveBeenCalledWith('/daily-logs/daily-log-1/learning-moments/moment-1');
  });
});

import type { Mock } from 'vitest';
import { apiClient } from '../../../shared/api/apiClient';
import type { GetHomeParams, HomeResponse } from '../types/home.types';
import { homeService } from './home.services';

vi.mock('../../../shared/api/apiClient', () => ({
  apiClient: {
    getSingle: vi.fn()
  }
}));

const apiClientMock = apiClient as unknown as {
  getSingle: Mock;
};

const homeResponse: HomeResponse = {
  children: [],
  metrics: [],
  recentActivities: [],
  today: {
    dailyLogCount: 0,
    learningMomentCount: 0,
    childrenObservedCount: 0
  },
  outcomeCoverage: {
    activeOutcomeCount: 0,
    touchedThisWeekCount: 0,
    untouchedActiveOutcomeCount: 0
  },
  recentMoments: []
};

describe('homeService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('gets home dashboard data', async () => {
    // Arrange
    const params: GetHomeParams = {
      today: '2026-07-16'
    };

    apiClientMock.getSingle.mockResolvedValue(homeResponse);

    // Act
    const result = await homeService.getHome(params);

    // Assert
    expect(result).toEqual(homeResponse);
    expect(apiClientMock.getSingle).toHaveBeenCalledTimes(1);
    expect(apiClientMock.getSingle).toHaveBeenCalledWith('/dashboard/home', params);
  });
});

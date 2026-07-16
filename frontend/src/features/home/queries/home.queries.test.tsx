import { waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import { renderHookWithClient } from '../../../testUtils/testQueryClientHelpers';
import { homeService } from '../services/home.services';
import type { HomeModel } from '../types/home.types';
import { homeKeys, useHomeQuery } from './home.queries';

vi.mock('../services/home.services', () => ({
  homeService: {
    getHome: vi.fn()
  }
}));

const homeServiceMock = homeService as unknown as {
  getHome: Mock;
};

const home: HomeModel = {
  children: [],
  metrics: [],
  recentActivities: [],
  today: {
    dailyLogCount: 1,
    learningMomentCount: 3,
    childrenObservedCount: 1
  },
  outcomeCoverage: {
    activeOutcomeCount: 12,
    touchedThisWeekCount: 5,
    untouchedActiveOutcomeCount: 7
  },
  recentMoments: []
};

describe('home queries', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('builds the current home query key with the requested date', () => {
    // Act
    const key = homeKeys.current({ today: '2026-07-16' });

    // Assert
    expect(key).toEqual(['home', '2026-07-16']);
  });

  test('builds the current home query key without a date', () => {
    // Act
    const key = homeKeys.current({});

    // Assert
    expect(key).toEqual(['home', null]);
  });

  test('loads home dashboard data', async () => {
    // Arrange
    const params = { today: '2026-07-16' };
    homeServiceMock.getHome.mockResolvedValue(home);

    // Act
    const { result } = renderHookWithClient(() => useHomeQuery(params));

    // Assert
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    expect(homeServiceMock.getHome).toHaveBeenCalledTimes(1);
    expect(homeServiceMock.getHome).toHaveBeenCalledWith(params);
    expect(result.current.data).toEqual(home);
  });
});

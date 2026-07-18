import type { HomeResponse } from '../types/home.types';
import {
  mapHomeChildResponseToModel,
  mapHomeMetricResponseToModel,
  mapHomeOutcomeCoverageResponseToModel,
  mapHomeRecentActivityResponseToModel,
  mapHomeRecentMomentResponseToModel,
  mapHomeResponseToModel,
  mapHomeTodaySummaryResponseToModel
} from './home.mapper';

const homeResponse: HomeResponse = {
  children: [
    {
      childId: 'child-1',
      givenName: 'Mia',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: 'stored-file-1'
    }
  ],
  metrics: [
    {
      label: 'Open invites',
      value: 2,
      detail: 'Invitations waiting for a response.'
    }
  ],
  recentActivities: [
    {
      dailyLogId: 'daily-log-1',
      childId: 'child-1',
      logDate: '2026-07-16',
      learningMomentCount: 3
    }
  ],
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
  recentMoments: [
    {
      dailyLogId: 'daily-log-1',
      learningMomentId: 'moment-1',
      childId: 'child-1',
      childName: 'Mia',
      logDate: '2026-07-16',
      kind: 'reading',
      title: 'Read a story',
      notes: 'Read a picture book and named familiar animals.',
      outcomeNames: ['Listens and responds']
    }
  ]
};

describe('home mapper', () => {
  test('maps home child response to model', () => {
    // Act
    const model = mapHomeChildResponseToModel(homeResponse.children[0]);

    // Assert
    expect(model).toEqual(homeResponse.children[0]);
  });

  test('maps home metric response to model', () => {
    // Act
    const model = mapHomeMetricResponseToModel(homeResponse.metrics[0]);

    // Assert
    expect(model).toEqual(homeResponse.metrics[0]);
  });

  test('maps home recent activity response to model', () => {
    // Act
    const model = mapHomeRecentActivityResponseToModel(homeResponse.recentActivities[0]);

    // Assert
    expect(model).toEqual(homeResponse.recentActivities[0]);
  });

  test('maps home today summary response to model', () => {
    // Act
    const model = mapHomeTodaySummaryResponseToModel(homeResponse.today);

    // Assert
    expect(model).toEqual(homeResponse.today);
  });

  test('maps home outcome coverage response to model', () => {
    // Act
    const model = mapHomeOutcomeCoverageResponseToModel(homeResponse.outcomeCoverage);

    // Assert
    expect(model).toEqual(homeResponse.outcomeCoverage);
  });

  test('maps home recent moment response to model', () => {
    // Act
    const model = mapHomeRecentMomentResponseToModel(homeResponse.recentMoments[0]);

    // Assert
    expect(model).toEqual(homeResponse.recentMoments[0]);
  });

  test('maps a home response to a home model', () => {
    // Act
    const model = mapHomeResponseToModel(homeResponse);

    // Assert
    expect(model).toEqual(homeResponse);
  });
});

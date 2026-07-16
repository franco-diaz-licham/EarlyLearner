import type { DailyLogResponse, LearningMomentFeedResponse } from '../types/dailyLog.types';
import { mapDailyLogResponseToModel, mapDailyLogResponsesToModels, mapLearningMomentFeedResponseToModel } from './dailyLog.mapper';

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

describe('daily log mapper', () => {
  test('maps a daily log response to a daily log model', () => {
    // Act
    const model = mapDailyLogResponseToModel(dailyLogResponse);

    // Assert
    expect(model).toEqual({
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
    });
  });

  test('maps daily log responses to daily log models', () => {
    // Arrange
    const secondDailyLogResponse: DailyLogResponse = {
      ...dailyLogResponse,
      dailyLogId: 'daily-log-2',
      learningMomentCount: 0,
      learningMoments: []
    };

    // Act
    const models = mapDailyLogResponsesToModels([dailyLogResponse, secondDailyLogResponse]);

    // Assert
    expect(models).toEqual([mapDailyLogResponseToModel(dailyLogResponse), mapDailyLogResponseToModel(secondDailyLogResponse)]);
  });

  test('maps a learning moment feed response to a learning moment feed model', () => {
    // Act
    const model = mapLearningMomentFeedResponseToModel(learningMomentFeedResponse);

    // Assert
    expect(model).toEqual({
      learningMomentId: 'moment-1',
      dailyLogId: 'daily-log-1',
      householdId: 'household-1',
      childId: 'child-1',
      logDate: '2026-07-16',
      kind: 'reading',
      title: 'Read a story',
      notes: 'Read a picture book and named familiar animals.',
      learningOutcomeIds: ['outcome-1']
    });
  });
});

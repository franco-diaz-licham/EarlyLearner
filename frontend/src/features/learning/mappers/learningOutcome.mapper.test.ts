import type { LearningOutcomeResponse } from '../types/learningOutcome.types';
import { LearningOutcomeStatus } from '../types/learningOutcome.types';
import { mapLearningOutcomeResponseToModel, mapLearningOutcomeResponsesToModels } from './learningOutcome.mapper';

const learningOutcomeResponse: LearningOutcomeResponse = {
  learningOutcomeId: 'outcome-1',
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10,
  status: LearningOutcomeStatus.Active
};

describe('learning outcome mapper', () => {
  test('maps a learning outcome response to a learning outcome model', () => {
    // Act
    const model = mapLearningOutcomeResponseToModel(learningOutcomeResponse);

    // Assert
    expect(model).toEqual({
      learningOutcomeId: 'outcome-1',
      code: 'language-listening',
      name: 'Listens and responds',
      description: 'Responds to familiar sounds, words, and stories.',
      category: 'Language',
      sortOrder: 10,
      status: LearningOutcomeStatus.Active
    });
  });

  test('maps learning outcome responses to learning outcome models', () => {
    // Arrange
    const secondLearningOutcomeResponse: LearningOutcomeResponse = {
      ...learningOutcomeResponse,
      learningOutcomeId: 'outcome-2',
      code: 'social-turn-taking',
      name: 'Takes turns'
    };

    // Act
    const models = mapLearningOutcomeResponsesToModels([learningOutcomeResponse, secondLearningOutcomeResponse]);

    // Assert
    expect(models).toEqual([mapLearningOutcomeResponseToModel(learningOutcomeResponse), mapLearningOutcomeResponseToModel(secondLearningOutcomeResponse)]);
  });
});

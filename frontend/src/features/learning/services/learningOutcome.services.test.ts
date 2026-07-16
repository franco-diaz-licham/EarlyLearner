import type { Mock } from 'vitest';
import { apiClient } from '../../../shared/api/apiClient';
import type { CreateLearningOutcomeRequest, LearningOutcomeResponse, UpdateLearningOutcomeRequest, UpdateLearningOutcomeStatusRequest } from '../types/learningOutcome.types';
import { LearningOutcomeStatus } from '../types/learningOutcome.types';
import { learningOutcomeService } from './learningOutcome.services';

vi.mock('../../../shared/api/apiClient', () => ({
  apiClient: {
    getList: vi.fn(),
    getSingle: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    patch: vi.fn(),
    delete: vi.fn()
  }
}));

const apiClientMock = apiClient as unknown as {
  getList: Mock;
  getSingle: Mock;
  post: Mock;
  put: Mock;
  patch: Mock;
  delete: Mock;
};

const learningOutcomeResponse: LearningOutcomeResponse = {
  learningOutcomeId: 'outcome-1',
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10,
  status: LearningOutcomeStatus.Active
};

describe('learningOutcomeService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('lists learning outcomes', async () => {
    // Arrange
    apiClientMock.getList.mockResolvedValue([learningOutcomeResponse]);

    // Act
    const result = await learningOutcomeService.list();

    // Assert
    expect(result).toEqual([learningOutcomeResponse]);
    expect(apiClientMock.getList).toHaveBeenCalledTimes(1);
    expect(apiClientMock.getList).toHaveBeenCalledWith('/learning-outcomes/');
  });

  test('gets a learning outcome', async () => {
    // Arrange
    apiClientMock.getSingle.mockResolvedValue(learningOutcomeResponse);

    // Act
    const result = await learningOutcomeService.get('outcome-1');

    // Assert
    expect(result).toEqual(learningOutcomeResponse);
    expect(apiClientMock.getSingle).toHaveBeenCalledTimes(1);
    expect(apiClientMock.getSingle).toHaveBeenCalledWith('/learning-outcomes/outcome-1');
  });

  test('creates a learning outcome', async () => {
    // Arrange
    const request: CreateLearningOutcomeRequest = {
      code: 'language-listening',
      name: 'Listens and responds',
      description: 'Responds to familiar sounds, words, and stories.',
      category: 'Language',
      sortOrder: 10
    };

    apiClientMock.post.mockResolvedValue(learningOutcomeResponse);

    // Act
    const result = await learningOutcomeService.create(request);

    // Assert
    expect(result).toEqual(learningOutcomeResponse);
    expect(apiClientMock.post).toHaveBeenCalledTimes(1);
    expect(apiClientMock.post).toHaveBeenCalledWith('/learning-outcomes/', request);
  });

  test('updates a learning outcome', async () => {
    // Arrange
    const request: UpdateLearningOutcomeRequest = {
      name: 'Updated outcome',
      description: 'Updated description.',
      category: 'Language',
      sortOrder: 20
    };

    apiClientMock.put.mockResolvedValue(learningOutcomeResponse);

    // Act
    const result = await learningOutcomeService.update('outcome-1', request);

    // Assert
    expect(result).toEqual(learningOutcomeResponse);
    expect(apiClientMock.put).toHaveBeenCalledTimes(1);
    expect(apiClientMock.put).toHaveBeenCalledWith('/learning-outcomes/outcome-1', request);
  });

  test('updates a learning outcome status', async () => {
    // Arrange
    const request: UpdateLearningOutcomeStatusRequest = {
      status: LearningOutcomeStatus.Inactive
    };

    apiClientMock.patch.mockResolvedValue(learningOutcomeResponse);

    // Act
    const result = await learningOutcomeService.updateStatus('outcome-1', request);

    // Assert
    expect(result).toEqual(learningOutcomeResponse);
    expect(apiClientMock.patch).toHaveBeenCalledTimes(1);
    expect(apiClientMock.patch).toHaveBeenCalledWith('/learning-outcomes/outcome-1/status', request);
  });

  test('deletes a learning outcome', async () => {
    // Arrange
    apiClientMock.delete.mockResolvedValue(undefined);

    // Act
    await learningOutcomeService.delete('outcome-1');

    // Assert
    expect(apiClientMock.delete).toHaveBeenCalledTimes(1);
    expect(apiClientMock.delete).toHaveBeenCalledWith('/learning-outcomes/outcome-1');
  });
});

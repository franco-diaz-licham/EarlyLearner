import { act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import { renderHookWithClient } from '../../../testUtils/testQueryClientHelpers';
import { learningOutcomeService } from '../services/learningOutcome.services';
import type { LearningOutcomeModel } from '../types/learningOutcome.types';
import type { LearningOutcomeFormModel } from '../hooks/useLearningOutcomeForm';
import { LearningOutcomeStatus } from '../types/learningOutcome.types';
import {
  learningOutcomeKeys,
  useCreateLearningOutcomeMutation,
  useDeleteLearningOutcomeMutation,
  useLearningOutcomeQuery,
  useLearningOutcomesQuery,
  useUpdateLearningOutcomeMutation,
  useUpdateLearningOutcomeStatusMutation
} from './learningOutcome.queries';

vi.mock('../services/learningOutcome.services', () => ({
  learningOutcomeService: {
    list: vi.fn(),
    get: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    updateStatus: vi.fn(),
    delete: vi.fn()
  }
}));

interface LearningOutcomeServiceMock {
  list: Mock;
  get: Mock;
  create: Mock;
  update: Mock;
  updateStatus: Mock;
  delete: Mock;
}

const learningOutcomeServiceMock = learningOutcomeService as unknown as LearningOutcomeServiceMock;

const learningOutcome: LearningOutcomeModel = {
  learningOutcomeId: 'outcome-1',
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10,
  status: LearningOutcomeStatus.Active
};

const updatedLearningOutcome: LearningOutcomeModel = {
  ...learningOutcome,
  name: 'Updated outcome'
};

describe('learning outcome queries', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('loads the learning outcome list', async () => {
    // Arrange
    learningOutcomeServiceMock.list.mockResolvedValue([learningOutcome]);

    // Act
    const { result } = renderHookWithClient(() => useLearningOutcomesQuery());

    // Assert
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    expect(learningOutcomeServiceMock.list).toHaveBeenCalledTimes(1);
    expect(result.current.data).toEqual([learningOutcome]);
  });

  test('loads a learning outcome detail', async () => {
    // Arrange
    learningOutcomeServiceMock.get.mockResolvedValue(learningOutcome);

    // Act
    const { result } = renderHookWithClient(() => useLearningOutcomeQuery('outcome-1'));

    // Assert
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    expect(learningOutcomeServiceMock.get).toHaveBeenCalledTimes(1);
    expect(learningOutcomeServiceMock.get).toHaveBeenCalledWith('outcome-1');
    expect(result.current.data).toEqual(learningOutcome);
  });

  test('does not load a learning outcome detail without an id', () => {
    // Act
    const { result } = renderHookWithClient(() => useLearningOutcomeQuery(''));

    // Assert
    expect(result.current.fetchStatus).toBe('idle');
    expect(learningOutcomeServiceMock.get).not.toHaveBeenCalled();
  });

  test('creates a learning outcome and refreshes learning outcome query data', async () => {
    // Arrange
    const form: LearningOutcomeFormModel = {
      code: 'language-listening',
      name: 'Listens and responds',
      description: 'Responds to familiar sounds, words, and stories.',
      category: 'Language',
      sortOrder: 10
    };

    learningOutcomeServiceMock.create.mockResolvedValue(learningOutcome);

    const { queryClient, result } = renderHookWithClient(() => useCreateLearningOutcomeMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync(form);
    });

    // Assert
    expect(learningOutcomeServiceMock.create).toHaveBeenCalledWith({
      code: form.code,
      name: form.name,
      description: form.description,
      category: form.category,
      sortOrder: form.sortOrder
    });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: learningOutcomeKeys.lists() });
    expect(queryClient.getQueryData(learningOutcomeKeys.detail('outcome-1'))).toEqual(learningOutcome);
  });

  test('updates a learning outcome and refreshes learning outcome query data', async () => {
    // Arrange
    const form: LearningOutcomeFormModel = {
      code: 'language-listening',
      name: 'Updated outcome',
      description: 'Updated description.',
      category: 'Language',
      sortOrder: 20
    };

    learningOutcomeServiceMock.update.mockResolvedValue(updatedLearningOutcome);

    const { queryClient, result } = renderHookWithClient(() => useUpdateLearningOutcomeMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync({ learningOutcomeId: 'outcome-1', form });
    });

    // Assert
    expect(learningOutcomeServiceMock.update).toHaveBeenCalledWith('outcome-1', {
      name: form.name,
      description: form.description,
      category: form.category,
      sortOrder: form.sortOrder
    });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: learningOutcomeKeys.lists() });
    expect(queryClient.getQueryData(learningOutcomeKeys.detail('outcome-1'))).toEqual(updatedLearningOutcome);
  });

  test('updates a learning outcome status and refreshes learning outcome query data', async () => {
    // Arrange
    const status = LearningOutcomeStatus.Inactive;

    learningOutcomeServiceMock.updateStatus.mockResolvedValue(updatedLearningOutcome);

    const { queryClient, result } = renderHookWithClient(() => useUpdateLearningOutcomeStatusMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync({ learningOutcomeId: 'outcome-1', form });
    });

    // Assert
    expect(learningOutcomeServiceMock.updateStatus).toHaveBeenCalledWith('outcome-1', { status });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: learningOutcomeKeys.lists() });
    expect(queryClient.getQueryData(learningOutcomeKeys.detail('outcome-1'))).toEqual(updatedLearningOutcome);
  });

  test('deletes a learning outcome and removes cached detail data', async () => {
    // Arrange
    learningOutcomeServiceMock.delete.mockResolvedValue(undefined);

    const { queryClient, result } = renderHookWithClient(() => useDeleteLearningOutcomeMutation());
    queryClient.setQueryData(learningOutcomeKeys.detail('outcome-1'), learningOutcome);
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');
    const removeQueries = vi.spyOn(queryClient, 'removeQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync('outcome-1');
    });

    // Assert
    expect(learningOutcomeServiceMock.delete).toHaveBeenCalledWith('outcome-1');
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: learningOutcomeKeys.lists() });
    expect(removeQueries).toHaveBeenCalledWith({ queryKey: learningOutcomeKeys.detail('outcome-1') });
  });
});

import { act, renderHook } from '@testing-library/react';
import type { LearningOutcomeFormModel } from './useLearningOutcomeForm';
import { createEmptyLearningOutcomeForm, getLearningOutcomeForm, useLearningOutcomeForm } from './useLearningOutcomeForm';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';

const validOutcomeDraft: LearningOutcomeFormModel = {
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10
};

const learningOutcome: LearningOutcomeModel = {
  learningOutcomeId: 'outcome-1',
  ...validOutcomeDraft,
  status: LearningOutcomeStatus.Active
};

describe('useLearningOutcomeForm', () => {
  test('creates an empty learning outcome form', () => {
    // Act
    const draft = createEmptyLearningOutcomeForm();

    // Assert
    expect(draft).toEqual({
      code: '',
      name: '',
      description: '',
      category: '',
      sortOrder: 0
    });
  });

  test('gets a learning outcome form from a model', () => {
    // Act
    const draft = getLearningOutcomeForm(learningOutcome);

    // Assert
    expect(draft).toEqual(validOutcomeDraft);
  });

  test('starts with the default draft and no visible errors', () => {
    // Arrange
    const { result } = renderHook(() => useLearningOutcomeForm());

    // Act
    const form = result.current;

    // Assert
    expect(form.draft).toEqual(createEmptyLearningOutcomeForm());
    expect(form.errors).toEqual({});
    expect(form.isValid).toBe(false);
  });

  test('starts from an initial draft when one is provided', () => {
    // Arrange
    const { result } = renderHook(() => useLearningOutcomeForm(validOutcomeDraft));

    // Act
    const form = result.current;

    // Assert
    expect(form.draft).toEqual(validOutcomeDraft);
    expect(form.errors).toEqual({});
    expect(form.isValid).toBe(true);
  });

  test('updates a field and shows errors only after the field is touched', () => {
    // Arrange
    const { result } = renderHook(() => useLearningOutcomeForm(validOutcomeDraft));

    // Act
    act(() => {
      result.current.updateField('name', '');
    });

    // Assert
    expect(result.current.draft.name).toBe('');
    expect(result.current.errors).toEqual({
      name: 'Name is required.'
    });
    expect(result.current.isValid).toBe(false);
  });

  test('returns null and reveals all errors when submitted with invalid values', () => {
    // Arrange
    const { result } = renderHook(() => useLearningOutcomeForm());

    // Act
    let submittedForm: LearningOutcomeFormModel | null = null;
    act(() => {
      submittedForm = result.current.getValidForm();
    });

    // Assert
    expect(submittedForm).toBeNull();
    expect(result.current.errors).toEqual({
      code: 'Code is required.',
      name: 'Name is required.',
      description: 'Description is required.',
      category: 'Category is required.'
    });
  });

  test('returns the draft when submitted with valid values', () => {
    // Arrange
    const { result } = renderHook(() => useLearningOutcomeForm());

    // Act
    act(() => {
      result.current.updateField('code', validOutcomeDraft.code);
      result.current.updateField('name', validOutcomeDraft.name);
      result.current.updateField('description', validOutcomeDraft.description);
      result.current.updateField('category', validOutcomeDraft.category);
      result.current.updateField('sortOrder', validOutcomeDraft.sortOrder);
    });

    let submittedForm: LearningOutcomeFormModel | null = null;
    act(() => {
      submittedForm = result.current.getValidForm();
    });

    // Assert
    expect(submittedForm).toEqual(validOutcomeDraft);
    expect(result.current.errors).toEqual({});
    expect(result.current.isValid).toBe(true);
  });

  test('resets to the requested draft and clears visible errors', () => {
    // Arrange
    const { result } = renderHook(() => useLearningOutcomeForm());

    act(() => {
      result.current.updateField('name', '');
      result.current.getValidForm();
    });

    // Act
    act(() => {
      result.current.reset(validOutcomeDraft);
    });

    // Assert
    expect(result.current.draft).toEqual(validOutcomeDraft);
    expect(result.current.errors).toEqual({});
    expect(result.current.isValid).toBe(true);
  });
});

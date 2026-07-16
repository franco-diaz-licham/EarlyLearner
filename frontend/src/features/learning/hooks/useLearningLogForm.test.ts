import { act, renderHook } from '@testing-library/react';
import type { LearningLogFormModel } from '../types/dailyLog.types';
import { createEmptyLearningLogForm, useLearningLogForm } from './useLearningLogForm';

const validLogDraft: LearningLogFormModel = {
  childId: 'child-1',
  logDate: '2026-07-16',
  kind: 'activity',
  title: 'Paint mixing',
  notes: 'Mixed colours and described the changes.',
  learningOutcomeIds: ['outcome-1']
};

describe('useLearningLogForm', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date('2026-07-16T10:00:00.000Z'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  test('creates an empty learning log form for the requested child', () => {
    // Act
    const draft = createEmptyLearningLogForm('child-1');

    // Assert
    expect(draft).toEqual({
      childId: 'child-1',
      logDate: '2026-07-16',
      kind: 'activity',
      title: '',
      notes: '',
      learningOutcomeIds: []
    });
  });

  test('starts with the default draft and no visible errors', () => {
    // Arrange
    const { result } = renderHook(() => useLearningLogForm(createEmptyLearningLogForm('child-1')));

    // Act
    const form = result.current;

    // Assert
    expect(form.draft).toEqual(createEmptyLearningLogForm('child-1'));
    expect(form.errors).toEqual({});
    expect(form.isValid).toBe(false);
  });

  test('starts from an initial draft when one is provided', () => {
    // Arrange
    const { result } = renderHook(() => useLearningLogForm(validLogDraft));

    // Act
    const form = result.current;

    // Assert
    expect(form.draft).toEqual(validLogDraft);
    expect(form.errors).toEqual({});
    expect(form.isValid).toBe(true);
  });

  test('updates a field and shows errors only after the field is touched', () => {
    // Arrange
    const { result } = renderHook(() => useLearningLogForm(validLogDraft));

    // Act
    act(() => {
      result.current.updateField('title', '');
    });

    // Assert
    expect(result.current.draft.title).toBe('');
    expect(result.current.errors).toEqual({
      title: 'Title is required.'
    });
    expect(result.current.isValid).toBe(false);
  });

  test('returns null and reveals all errors when submitted with invalid values', () => {
    // Arrange
    const { result } = renderHook(() => useLearningLogForm(createEmptyLearningLogForm()));

    // Act
    let submittedForm: LearningLogFormModel | null = null;
    act(() => {
      submittedForm = result.current.getValidForm();
    });

    // Assert
    expect(submittedForm).toBeNull();
    expect(result.current.errors).toEqual({
      childId: 'Child is required.',
      title: 'Title is required.',
      notes: 'Notes are required.',
      learningOutcomeIds: 'At least one learning outcome is required.'
    });
  });

  test('returns the draft when submitted with valid values', () => {
    // Arrange
    const { result } = renderHook(() => useLearningLogForm());

    // Act
    act(() => {
      result.current.updateField('childId', validLogDraft.childId);
      result.current.updateField('logDate', validLogDraft.logDate);
      result.current.updateField('kind', validLogDraft.kind);
      result.current.updateField('title', validLogDraft.title);
      result.current.updateField('notes', validLogDraft.notes);
      result.current.updateField('learningOutcomeIds', validLogDraft.learningOutcomeIds);
    });

    let submittedForm: LearningLogFormModel | null = null;
    act(() => {
      submittedForm = result.current.getValidForm();
    });

    // Assert
    expect(submittedForm).toEqual(validLogDraft);
    expect(result.current.errors).toEqual({});
    expect(result.current.isValid).toBe(true);
  });

  test('resets to the requested draft and clears visible errors', () => {
    // Arrange
    const { result } = renderHook(() => useLearningLogForm());

    act(() => {
      result.current.updateField('title', '');
      result.current.getValidForm();
    });

    // Act
    act(() => {
      result.current.reset(validLogDraft);
    });

    // Assert
    expect(result.current.draft).toEqual(validLogDraft);
    expect(result.current.errors).toEqual({});
    expect(result.current.isValid).toBe(true);
  });
});

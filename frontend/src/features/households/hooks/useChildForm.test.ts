import { act, renderHook } from '@testing-library/react';
import type { AddChildForm } from '../types/household.types';
import { useAddChildForm } from './useChildForm';

const validChildDraft: AddChildForm = {
  firstName: 'Mia',
  lastName: 'Rivera',
  dateOfBirth: '2021-04-15',
  avatarStoredFileId: 'stored-file-1'
};

describe('useAddChildForm', () => {
  test('starts with the default draft and no visible errors', () => {
    // Arrange
    const { result } = renderHook(() => useAddChildForm());

    // Act
    const form = result.current;

    // Assert
    expect(form.draft).toEqual({
      firstName: '',
      lastName: '',
      dateOfBirth: '',
      avatarStoredFileId: null
    });
    expect(form.errors).toEqual({});
    expect(form.isValid).toBe(false);
  });

  test('starts from an initial draft when one is provided', () => {
    // Arrange
    const { result } = renderHook(() => useAddChildForm(validChildDraft));

    // Act
    const form = result.current;

    // Assert
    expect(form.draft).toEqual(validChildDraft);
    expect(form.errors).toEqual({});
    expect(form.isValid).toBe(true);
  });

  test('updates a field and shows errors only after the field is touched', () => {
    // Arrange
    const { result } = renderHook(() => useAddChildForm(validChildDraft));

    // Act
    act(() => {
      result.current.updateField('firstName', '');
    });

    // Assert
    expect(result.current.draft.firstName).toBe('');
    expect(result.current.errors).toEqual({
      firstName: 'First name is required.'
    });
    expect(result.current.isValid).toBe(false);
  });

  test('returns null and reveals all errors when submitted with invalid values', () => {
    // Arrange
    const { result } = renderHook(() => useAddChildForm());

    // Act
    let submittedForm: AddChildForm | null = null;
    act(() => {
      submittedForm = result.current.getValidForm();
    });

    // Assert
    expect(submittedForm).toBeNull();
    expect(result.current.errors).toEqual({
      firstName: 'First name is required.',
      lastName: 'Last name is required.',
      dateOfBirth: 'Date of birth is required.'
    });
  });

  test('returns the draft when submitted with valid values', () => {
    // Arrange
    const { result } = renderHook(() => useAddChildForm());

    // Act
    act(() => {
      result.current.updateField('firstName', validChildDraft.firstName);
      result.current.updateField('lastName', validChildDraft.lastName);
      result.current.updateField('dateOfBirth', validChildDraft.dateOfBirth);
      result.current.updateField('avatarStoredFileId', validChildDraft.avatarStoredFileId);
    });

    let submittedForm: AddChildForm | null = null;
    act(() => {
      submittedForm = result.current.getValidForm();
    });

    // Assert
    expect(submittedForm).toEqual(validChildDraft);
    expect(result.current.errors).toEqual({});
    expect(result.current.isValid).toBe(true);
  });

  test('resets to the requested draft and clears visible errors', () => {
    // Arrange
    const { result } = renderHook(() => useAddChildForm());

    act(() => {
      result.current.updateField('firstName', '');
      result.current.getValidForm();
    });

    // Act
    act(() => {
      result.current.reset(validChildDraft);
    });

    // Assert
    expect(result.current.draft).toEqual(validChildDraft);
    expect(result.current.errors).toEqual({});
    expect(result.current.isValid).toBe(true);
  });
});

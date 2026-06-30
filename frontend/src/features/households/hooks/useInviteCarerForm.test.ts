import { act, renderHook } from '@testing-library/react';
import type { InviteCarerForm } from '../types/household.types';
import { useInviteCarerForm } from './useInviteCarerForm';

describe('useInviteCarerForm', () => {
  test('starts with the default draft and no visible errors', () => {
    // Arrange
    const { result } = renderHook(() => useInviteCarerForm());

    // Act
    const form = result.current;

    // Assert
    expect(form.draft).toEqual({
      email: '',
      role: 'caregiver'
    });
    expect(form.errors).toEqual({});
    expect(form.isValid).toBe(false);
  });

  test('updates a field and shows errors only after the field is touched', () => {
    // Arrange
    const { result } = renderHook(() => useInviteCarerForm());

    // Act
    act(() => {
      result.current.updateField('email', 'not-an-email');
    });

    // Assert
    expect(result.current.draft.email).toBe('not-an-email');
    expect(result.current.errors).toEqual({
      email: 'Enter a valid email address.'
    });
    expect(result.current.isValid).toBe(false);
  });

  test('returns null and reveals errors when submitted with invalid values', () => {
    // Arrange
    const { result } = renderHook(() => useInviteCarerForm());

    // Act
    let submittedForm: InviteCarerForm | null = null;
    act(() => {
      submittedForm = result.current.getValidForm();
    });

    // Assert
    expect(submittedForm).toBeNull();
    expect(result.current.errors).toEqual({
      email: 'Enter a valid email address.'
    });
  });

  test('returns the draft when submitted with valid values', () => {
    // Arrange
    const { result } = renderHook(() => useInviteCarerForm());

    // Act
    act(() => {
      result.current.updateField('email', 'caregiver@example.com');
      result.current.updateField('role', 'viewer');
    });

    let submittedForm: InviteCarerForm | null = null;
    act(() => {
      submittedForm = result.current.getValidForm();
    });

    // Assert
    expect(submittedForm).toEqual({
      email: 'caregiver@example.com',
      role: 'viewer'
    });
    expect(result.current.errors).toEqual({});
    expect(result.current.isValid).toBe(true);
  });

  test('resets the draft and clears visible errors', () => {
    // Arrange
    const { result } = renderHook(() => useInviteCarerForm());

    act(() => {
      result.current.updateField('email', 'not-an-email');
    });

    // Act
    act(() => {
      result.current.reset();
    });

    // Assert
    expect(result.current.draft).toEqual({
      email: '',
      role: 'caregiver'
    });
    expect(result.current.errors).toEqual({});
    expect(result.current.isValid).toBe(false);
  });
});

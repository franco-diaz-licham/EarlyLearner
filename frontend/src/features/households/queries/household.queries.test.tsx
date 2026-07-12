import { act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import { renderHookWithClient } from '../../../testUtils/testQueryClientHelpers';
import type { AddHouseholdChildRequest, HouseholdResponse, InviteHouseholdCarerRequest, UpdateHouseholdChildRequest, UpdateHouseholdRequest } from '../types/household.api.types';
import type { AddChildForm, InviteCarerForm, RenameHouseholdForm } from '../types/household.types';
import { householdService } from '../services/household.services';
import {
  householdKeys,
  useAddHouseholdChildMutation,
  useHouseholdQuery,
  useHouseholdsQuery,
  useInviteHouseholdCarerMutation,
  useRemoveHouseholdCarerMutation,
  useRemoveHouseholdChildMutation,
  useUpdateHouseholdChildMutation,
  useUpdateHouseholdMutation
} from './household.queries';

vi.mock('../services/household.services', () => ({
  householdService: {
    list: vi.fn(),
    get: vi.fn(),
    update: vi.fn(),
    inviteCarer: vi.fn(),
    removeCarer: vi.fn(),
    addChild: vi.fn(),
    updateChild: vi.fn(),
    removeChild: vi.fn()
  }
}));

interface HouseholdServiceMock {
  list: Mock;
  get: Mock;
  update: Mock;
  inviteCarer: Mock;
  removeCarer: Mock;
  addChild: Mock;
  updateChild: Mock;
  removeChild: Mock;
}

const householdServiceMock = householdService as unknown as HouseholdServiceMock;

const householdResponse: HouseholdResponse = {
  id: 'household-1',
  name: 'Rivera Household',
  carers: [],
  children: [],
  invitations: []
};

const updatedHouseholdResponse: HouseholdResponse = {
  ...householdResponse,
  name: 'Updated Household'
};

describe('household queries', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('loads and maps the household list', async () => {
    // Arrange
    householdServiceMock.list.mockResolvedValue([householdResponse]);

    // Act
    const { result } = renderHookWithClient(() => useHouseholdsQuery());

    // Assert
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    expect(householdServiceMock.list).toHaveBeenCalledTimes(1);
    expect(result.current.data).toEqual([householdResponse]);
  });

  test('loads and maps the current household', async () => {
    // Arrange
    householdServiceMock.get.mockResolvedValue(householdResponse);

    // Act
    const { result } = renderHookWithClient(() => useHouseholdQuery());

    // Assert
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    expect(householdServiceMock.get).toHaveBeenCalledTimes(1);
    expect(result.current.data).toEqual(householdResponse);
  });

  test('updates a household and refreshes household query data', async () => {
    // Arrange
    const form: RenameHouseholdForm = {
      name: '  Updated Household  '
    };

    householdServiceMock.update.mockResolvedValue(updatedHouseholdResponse);

    const { queryClient, result } = renderHookWithClient(() => useUpdateHouseholdMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync(form);
    });

    // Assert
    expect(householdServiceMock.update).toHaveBeenCalledWith({
      name: 'Updated Household'
    } satisfies UpdateHouseholdRequest);
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: householdKeys.lists() });
    expect(queryClient.getQueryData(householdKeys.current())).toEqual(updatedHouseholdResponse);
  });

  test('invites a household carer and refreshes household query data', async () => {
    // Arrange
    const form: InviteCarerForm = {
      email: '  caregiver@example.com  ',
      role: 'caregiver'
    };

    householdServiceMock.inviteCarer.mockResolvedValue(updatedHouseholdResponse);

    const { queryClient, result } = renderHookWithClient(() => useInviteHouseholdCarerMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync(form);
    });

    // Assert
    expect(householdServiceMock.inviteCarer).toHaveBeenCalledWith({
      email: 'caregiver@example.com',
      role: 'caregiver'
    } satisfies InviteHouseholdCarerRequest);
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: householdKeys.lists() });
    expect(queryClient.getQueryData(householdKeys.list())).toEqual([updatedHouseholdResponse]);
    expect(queryClient.getQueryData(householdKeys.current())).toEqual(updatedHouseholdResponse);
  });

  test('removes a household carer and refreshes household query data', async () => {
    // Arrange
    householdServiceMock.removeCarer.mockResolvedValue(updatedHouseholdResponse);

    const { queryClient, result } = renderHookWithClient(() => useRemoveHouseholdCarerMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync('carer-1');
    });

    // Assert
    expect(householdServiceMock.removeCarer).toHaveBeenCalledWith('carer-1');
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: householdKeys.lists() });
    expect(queryClient.getQueryData(householdKeys.current())).toEqual(updatedHouseholdResponse);
  });

  test('adds a household child and refreshes household query data', async () => {
    // Arrange
    const form: AddChildForm = {
      firstName: '  Mia  ',
      lastName: '  Rivera  ',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: null
    };

    householdServiceMock.addChild.mockResolvedValue(updatedHouseholdResponse);

    const { queryClient, result } = renderHookWithClient(() => useAddHouseholdChildMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync(form);
    });

    // Assert
    expect(householdServiceMock.addChild).toHaveBeenCalledWith({
      firstName: 'Mia',
      lastName: 'Rivera',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: null
    } satisfies AddHouseholdChildRequest);
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: householdKeys.lists() });
    expect(queryClient.getQueryData(householdKeys.current())).toEqual(updatedHouseholdResponse);
  });

  test('updates a household child and refreshes household query data', async () => {
    // Arrange
    const form: AddChildForm = {
      firstName: '  Mia  ',
      lastName: '  Rivera  ',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: 'stored-file-1'
    };

    householdServiceMock.updateChild.mockResolvedValue(updatedHouseholdResponse);

    const { queryClient, result } = renderHookWithClient(() => useUpdateHouseholdChildMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync({ childId: 'child-1', form });
    });

    // Assert
    expect(householdServiceMock.updateChild).toHaveBeenCalledWith('child-1', {
      firstName: 'Mia',
      lastName: 'Rivera',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: 'stored-file-1'
    } satisfies UpdateHouseholdChildRequest);
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: householdKeys.lists() });
    expect(queryClient.getQueryData(householdKeys.current())).toEqual(updatedHouseholdResponse);
  });

  test('removes a household child and refreshes household query data', async () => {
    // Arrange
    householdServiceMock.removeChild.mockResolvedValue(updatedHouseholdResponse);

    const { queryClient, result } = renderHookWithClient(() => useRemoveHouseholdChildMutation());
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

    // Act
    await act(async () => {
      await result.current.mutateAsync('child-1');
    });

    // Assert
    expect(householdServiceMock.removeChild).toHaveBeenCalledWith('child-1');
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: householdKeys.lists() });
    expect(queryClient.getQueryData(householdKeys.current())).toEqual(updatedHouseholdResponse);
  });
});

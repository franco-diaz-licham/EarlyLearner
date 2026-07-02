import type { Mock } from 'vitest';
import { apiClient } from '../../../shared/api/apiClient';
import type { AddHouseholdChildRequest, HouseholdResponse, InviteHouseholdCarerRequest, UpdateHouseholdChildRequest, UpdateHouseholdRequest } from '../types/household.api.types';
import { householdService } from './household.services';

vi.mock('../../../shared/api/apiClient', () => ({
  apiClient: {
    getList: vi.fn(),
    getSingle: vi.fn(),
    put: vi.fn(),
    post: vi.fn(),
    deleteResult: vi.fn()
  }
}));

const apiClientMock = apiClient as unknown as {
  getList: Mock;
  getSingle: Mock;
  put: Mock;
  post: Mock;
  deleteResult: Mock;
};

const householdResponse: HouseholdResponse = {
  id: 'household-1',
  name: 'Rivera Household',
  carers: [],
  children: [],
  invitations: []
};

describe('householdService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('lists households', async () => {
    // Arrange
    apiClientMock.getList.mockResolvedValue([householdResponse]);

    // Act
    const result = await householdService.list();

    // Assert
    expect(result).toEqual([householdResponse]);
    expect(apiClientMock.getList).toHaveBeenCalledTimes(1);
    expect(apiClientMock.getList).toHaveBeenCalledWith('/households');
  });

  test('gets the current household', async () => {
    // Arrange
    apiClientMock.getSingle.mockResolvedValue(householdResponse);

    // Act
    const result = await householdService.get();

    // Assert
    expect(result).toEqual(householdResponse);
    expect(apiClientMock.getSingle).toHaveBeenCalledTimes(1);
    expect(apiClientMock.getSingle).toHaveBeenCalledWith('/households/current');
  });

  test('updates the current household', async () => {
    // Arrange
    const request: UpdateHouseholdRequest = {
      name: 'Updated Household'
    };

    apiClientMock.put.mockResolvedValue(householdResponse);

    // Act
    const result = await householdService.update(request);

    // Assert
    expect(result).toEqual(householdResponse);
    expect(apiClientMock.put).toHaveBeenCalledTimes(1);
    expect(apiClientMock.put).toHaveBeenCalledWith('/households', request);
  });

  test('invites a carer', async () => {
    // Arrange
    const request: InviteHouseholdCarerRequest = {
      email: 'caregiver@example.com',
      role: 'caregiver'
    };

    apiClientMock.post.mockResolvedValue(householdResponse);

    // Act
    const result = await householdService.inviteCarer(request);

    // Assert
    expect(result).toEqual(householdResponse);
    expect(apiClientMock.post).toHaveBeenCalledTimes(1);
    expect(apiClientMock.post).toHaveBeenCalledWith('/households/carer-invitations', request);
  });

  test('removes a carer', async () => {
    // Arrange
    apiClientMock.deleteResult.mockResolvedValue(householdResponse);

    // Act
    const result = await householdService.removeCarer('carer-1');

    // Assert
    expect(result).toEqual(householdResponse);
    expect(apiClientMock.deleteResult).toHaveBeenCalledTimes(1);
    expect(apiClientMock.deleteResult).toHaveBeenCalledWith('/households/carers/carer-1');
  });

  test('adds a child', async () => {
    // Arrange
    const request: AddHouseholdChildRequest = {
      firstName: 'Mia',
      lastName: 'Rivera',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: null
    };

    apiClientMock.post.mockResolvedValue(householdResponse);

    // Act
    const result = await householdService.addChild(request);

    // Assert
    expect(result).toEqual(householdResponse);
    expect(apiClientMock.post).toHaveBeenCalledTimes(1);
    expect(apiClientMock.post).toHaveBeenCalledWith('/households/children', request);
  });

  test('updates a child', async () => {
    // Arrange
    const request: UpdateHouseholdChildRequest = {
      firstName: 'Mia',
      lastName: 'Rivera',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: 'stored-file-1'
    };

    apiClientMock.put.mockResolvedValue(householdResponse);

    // Act
    const result = await householdService.updateChild('child-1', request);

    // Assert
    expect(result).toEqual(householdResponse);
    expect(apiClientMock.put).toHaveBeenCalledTimes(1);
    expect(apiClientMock.put).toHaveBeenCalledWith('/households/children/child-1', request);
  });

  test('removes a child', async () => {
    // Arrange
    apiClientMock.deleteResult.mockResolvedValue(householdResponse);

    // Act
    const result = await householdService.removeChild('child-1');

    // Assert
    expect(result).toEqual(householdResponse);
    expect(apiClientMock.deleteResult).toHaveBeenCalledTimes(1);
    expect(apiClientMock.deleteResult).toHaveBeenCalledWith('/households/children/child-1');
  });
});

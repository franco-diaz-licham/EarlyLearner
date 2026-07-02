import type { HouseholdResponse } from '../types/household.api.types';
import type { AddChildForm, InviteCarerForm, RenameHouseholdForm } from '../types/household.types';
import { mapAddChildFormToRequest, mapHouseholdResponseToModel, mapHouseholdResponsesToModels, mapInviteCarerFormToRequest, mapRenameHouseholdFormToRequest, mapUpdateChildFormToRequest } from './household.mapper';

const householdResponse: HouseholdResponse = {
  id: 'household-1',
  name: 'Rivera Household',
  carers: [
    {
      id: 'carer-1',
      userId: 'user-1',
      email: 'alex@example.com',
      firstName: 'Alex',
      lastName: 'Rivera',
      role: 'owner',
      accountStatus: 'active'
    }
  ],
  children: [
    {
      id: 'child-1',
      firstName: 'Mia',
      lastName: 'Rivera',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: 'stored-file-1'
    }
  ],
  invitations: [
    {
      id: 'invitation-1',
      email: 'viewer@example.com',
      firstName: null,
      lastName: null,
      role: 'viewer',
      status: 'pending',
      invitedAt: '2026-01-01T00:00:00Z',
      expiresAt: '2026-01-08T00:00:00Z'
    }
  ]
};

describe('household mapper', () => {
  test('maps a household response to a household model', () => {
    // Act
    const model = mapHouseholdResponseToModel(householdResponse);

    // Assert
    expect(model).toEqual({
      id: 'household-1',
      name: 'Rivera Household',
      carers: householdResponse.carers,
      children: householdResponse.children,
      invitations: householdResponse.invitations
    });
  });

  test('maps household responses to household models', () => {
    // Arrange
    const secondHouseholdResponse: HouseholdResponse = {
      ...householdResponse,
      id: 'household-2',
      name: 'Chen Household'
    };

    // Act
    const models = mapHouseholdResponsesToModels([householdResponse, secondHouseholdResponse]);

    // Assert
    expect(models).toEqual([mapHouseholdResponseToModel(householdResponse), mapHouseholdResponseToModel(secondHouseholdResponse)]);
  });

  test('maps a rename household form to a trimmed request', () => {
    // Arrange
    const form: RenameHouseholdForm = {
      name: '  Rivera Household  '
    };

    // Act
    const request = mapRenameHouseholdFormToRequest(form);

    // Assert
    expect(request).toEqual({
      name: 'Rivera Household'
    });
  });

  test('maps an invite carer form to a trimmed request', () => {
    // Arrange
    const form: InviteCarerForm = {
      email: '  caregiver@example.com  ',
      role: 'caregiver'
    };

    // Act
    const request = mapInviteCarerFormToRequest(form);

    // Assert
    expect(request).toEqual({
      email: 'caregiver@example.com',
      role: 'caregiver'
    });
  });

  test('maps an add child form to a trimmed request', () => {
    // Arrange
    const form: AddChildForm = {
      firstName: '  Mia  ',
      lastName: '  Rivera  ',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: 'stored-file-1'
    };

    // Act
    const request = mapAddChildFormToRequest(form);

    // Assert
    expect(request).toEqual({
      firstName: 'Mia',
      lastName: 'Rivera',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: 'stored-file-1'
    });
  });

  test('maps an update child form to a trimmed request', () => {
    // Arrange
    const form: AddChildForm = {
      firstName: '  Noah  ',
      lastName: '  Rivera  ',
      dateOfBirth: '2020-09-02',
      avatarStoredFileId: null
    };

    // Act
    const request = mapUpdateChildFormToRequest(form);

    // Assert
    expect(request).toEqual({
      firstName: 'Noah',
      lastName: 'Rivera',
      dateOfBirth: '2020-09-02',
      avatarStoredFileId: null
    });
  });
});

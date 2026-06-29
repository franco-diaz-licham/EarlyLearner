import type { AddHouseholdChildRequest, HouseholdResponse, InviteHouseholdCarerRequest, UpdateHouseholdChildRequest, UpdateHouseholdRequest } from '../types/household.api.types';
import type { AddChildForm, HouseholdModel, InviteCarerForm, RenameHouseholdForm } from '../types/household.types';

export const mapHouseholdResponseToModel = (response: HouseholdResponse): HouseholdModel => ({
  id: response.id,
  name: response.name,
  carers: response.carers,
  children: response.children,
  invitations: response.invitations
});

export const mapHouseholdResponsesToModels = (responses: HouseholdResponse[]): HouseholdModel[] => responses.map(mapHouseholdResponseToModel);

export const mapRenameHouseholdFormToRequest = (form: RenameHouseholdForm): UpdateHouseholdRequest => ({
  name: form.name.trim()
});

export const mapInviteCarerFormToRequest = (form: InviteCarerForm): InviteHouseholdCarerRequest => ({
  email: form.email.trim(),
  role: form.role
});

export const mapAddChildFormToRequest = (form: AddChildForm): AddHouseholdChildRequest => ({
  firstName: form.firstName.trim(),
  lastName: form.lastName.trim(),
  dateOfBirth: form.dateOfBirth,
  avatarStoredFileId: form.avatarStoredFileId
});

export const mapUpdateChildFormToRequest = (form: AddChildForm): UpdateHouseholdChildRequest => ({
  firstName: form.firstName.trim(),
  lastName: form.lastName.trim(),
  dateOfBirth: form.dateOfBirth,
  avatarStoredFileId: form.avatarStoredFileId
});

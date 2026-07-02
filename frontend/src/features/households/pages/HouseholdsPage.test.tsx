import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { StoredFileMediaType } from '../../stored-files/types/storedFile.types';
import * as fileUploadQueries from '../../../shared/queries/fileUpload.queries';
import type { ChildModel, HouseholdModel } from '../types/household.types';
import * as householdQueries from '../queries/household.queries';
import { HouseholdsPage } from './HouseholdsPage';

vi.mock('../queries/household.queries', () => ({
  useAddHouseholdChildMutation: vi.fn(),
  useHouseholdsQuery: vi.fn(),
  useInviteHouseholdCarerMutation: vi.fn(),
  useRemoveHouseholdCarerMutation: vi.fn(),
  useRemoveHouseholdChildMutation: vi.fn(),
  useUpdateHouseholdChildMutation: vi.fn(),
  useUpdateHouseholdMutation: vi.fn()
}));

vi.mock('../../../shared/queries/fileUpload.queries', () => ({
  useCreateStoredFileUploadMutation: vi.fn(),
  useDeleteStoredFileUploadMutation: vi.fn(),
  useStoredFileContentQuery: vi.fn()
}));

const child: ChildModel = {
  id: 'child-1',
  firstName: 'Mia',
  lastName: 'Rivera',
  dateOfBirth: '2021-04-15',
  avatarStoredFileId: 'stored-file-old'
};

const household: HouseholdModel = {
  id: 'household-1',
  name: 'Rivera Household',
  carers: [
    {
      id: 'carer-owner',
      userId: 'user-owner',
      email: 'owner@example.com',
      firstName: 'Olivia',
      lastName: 'Rivera',
      role: 'Owner',
      accountStatus: 'Active'
    },
    {
      id: 'carer-caregiver',
      userId: 'user-caregiver',
      email: 'caregiver@example.com',
      firstName: 'Sam',
      lastName: 'Taylor',
      role: 'Caregiver',
      accountStatus: 'Invited'
    }
  ],
  children: [child],
  invitations: [
    {
      id: 'invitation-1',
      email: 'viewer@example.com',
      firstName: null,
      lastName: null,
      role: 'Viewer',
      status: 'Pending',
      invitedAt: '2026-06-01T00:00:00.000Z',
      expiresAt: '2026-07-01T00:00:00.000Z'
    }
  ]
};

const householdsQuery = {
  data: [household],
  isError: false,
  isLoading: false
};

const updateHouseholdMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const inviteHouseholdCarerMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const removeHouseholdCarerMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const addHouseholdChildMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const updateHouseholdChildMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const removeHouseholdChildMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const createStoredFileUploadMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const deleteStoredFileUploadMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const renderPage = () => render(<HouseholdsPage />);

describe('HouseholdsPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();

    vi.mocked(householdQueries.useHouseholdsQuery).mockReturnValue(householdsQuery as ReturnType<typeof householdQueries.useHouseholdsQuery>);
    vi.mocked(householdQueries.useUpdateHouseholdMutation).mockReturnValue(updateHouseholdMutation as unknown as ReturnType<typeof householdQueries.useUpdateHouseholdMutation>);
    vi.mocked(householdQueries.useInviteHouseholdCarerMutation).mockReturnValue(inviteHouseholdCarerMutation as unknown as ReturnType<typeof householdQueries.useInviteHouseholdCarerMutation>);
    vi.mocked(householdQueries.useRemoveHouseholdCarerMutation).mockReturnValue(removeHouseholdCarerMutation as unknown as ReturnType<typeof householdQueries.useRemoveHouseholdCarerMutation>);
    vi.mocked(householdQueries.useAddHouseholdChildMutation).mockReturnValue(addHouseholdChildMutation as unknown as ReturnType<typeof householdQueries.useAddHouseholdChildMutation>);
    vi.mocked(householdQueries.useUpdateHouseholdChildMutation).mockReturnValue(updateHouseholdChildMutation as unknown as ReturnType<typeof householdQueries.useUpdateHouseholdChildMutation>);
    vi.mocked(householdQueries.useRemoveHouseholdChildMutation).mockReturnValue(removeHouseholdChildMutation as unknown as ReturnType<typeof householdQueries.useRemoveHouseholdChildMutation>);

    vi.mocked(fileUploadQueries.useCreateStoredFileUploadMutation).mockReturnValue(createStoredFileUploadMutation as unknown as ReturnType<typeof fileUploadQueries.useCreateStoredFileUploadMutation>);
    vi.mocked(fileUploadQueries.useDeleteStoredFileUploadMutation).mockReturnValue(deleteStoredFileUploadMutation as unknown as ReturnType<typeof fileUploadQueries.useDeleteStoredFileUploadMutation>);
    vi.mocked(fileUploadQueries.useStoredFileContentQuery).mockReturnValue({ data: null } as unknown as ReturnType<typeof fileUploadQueries.useStoredFileContentQuery>);

    updateHouseholdMutation.mutateAsync.mockResolvedValue(household);
    inviteHouseholdCarerMutation.mutateAsync.mockResolvedValue(household);
    removeHouseholdCarerMutation.mutateAsync.mockResolvedValue(household);
    addHouseholdChildMutation.mutateAsync.mockResolvedValue(household);
    updateHouseholdChildMutation.mutateAsync.mockResolvedValue(household);
    removeHouseholdChildMutation.mutateAsync.mockResolvedValue(household);
    createStoredFileUploadMutation.mutateAsync.mockResolvedValue({ storedFileId: 'stored-file-new' });
    deleteStoredFileUploadMutation.mutateAsync.mockResolvedValue(undefined);

    vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:avatar-preview');
    vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => undefined);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  test('renders the household dashboard from the household query', () => {
    // Act
    renderPage();

    // Assert
    expect(screen.getByRole('heading', { name: 'Rivera Household' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Household Members' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Mia Rivera' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Sam Taylor' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'viewer@example.com' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Household Summary' })).toBeInTheDocument();
    expect(screen.getByText('1 child profile and 1 invitation.')).toBeInTheDocument();
  });

  test('renames the current household', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Rename Rivera Household' }));
    await user.clear(screen.getByLabelText('Household name'));
    await user.type(screen.getByLabelText('Household name'), 'Rivera Family');
    await user.click(screen.getByRole('button', { name: 'Save' }));

    // Assert
    expect(updateHouseholdMutation.mutateAsync).toHaveBeenCalledTimes(1);
    expect(updateHouseholdMutation.mutateAsync).toHaveBeenCalledWith({ name: 'Rivera Family' });
  });

  test('opens the invite form and submits a carer invite', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Invite carer' }));
    await user.type(screen.getByLabelText(/email/i), 'caregiver2@example.com');
    await user.click(screen.getByRole('button', { name: 'Invite' }));

    // Assert
    expect(screen.getByRole('dialog', { name: 'Invite to Rivera Household' })).toBeInTheDocument();
    expect(inviteHouseholdCarerMutation.mutateAsync).toHaveBeenCalledTimes(1);
    expect(inviteHouseholdCarerMutation.mutateAsync).toHaveBeenCalledWith({
      email: 'caregiver2@example.com',
      role: 'caregiver'
    });
  });

  test('opens the add child form, uploads an avatar, and adds the child', async () => {
    // Arrange
    const user = userEvent.setup();
    const avatarFile = new File(['avatar'], 'avatar.png', { type: 'image/png' });

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Add child' }));
    await user.type(screen.getByLabelText(/first name/i), 'Noah');
    await user.type(screen.getByLabelText(/last name/i), 'Rivera');
    await user.type(screen.getByLabelText(/date of birth/i), '2020-09-02');
    await user.upload(screen.getByLabelText('Upload image'), avatarFile);
    await user.click(within(screen.getByRole('dialog', { name: 'Add child in Rivera Household' })).getByRole('button', { name: 'Add child' }));

    // Assert
    expect(createStoredFileUploadMutation.mutateAsync).toHaveBeenCalledWith({
      file: avatarFile,
      mediaType: StoredFileMediaType.Photo
    });
    expect(addHouseholdChildMutation.mutateAsync).toHaveBeenCalledWith({
      firstName: 'Noah',
      lastName: 'Rivera',
      dateOfBirth: '2020-09-02',
      avatarStoredFileId: 'stored-file-new'
    });
  });

  test('edits a child, replaces the avatar, and deletes the replaced avatar', async () => {
    // Arrange
    const user = userEvent.setup();
    const avatarFile = new File(['updated avatar'], 'updated-avatar.png', { type: 'image/png' });

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Edit Mia Rivera' }));
    await user.clear(screen.getByLabelText(/first name/i));
    await user.type(screen.getByLabelText(/first name/i), 'Ava');
    await user.upload(screen.getByLabelText('Upload image'), avatarFile);
    await user.click(screen.getByRole('button', { name: 'Save child' }));

    // Assert
    expect(updateHouseholdChildMutation.mutateAsync).toHaveBeenCalledWith({
      childId: 'child-1',
      form: {
        firstName: 'Ava',
        lastName: 'Rivera',
        dateOfBirth: '2021-04-15',
        avatarStoredFileId: 'stored-file-new'
      }
    });
    expect(deleteStoredFileUploadMutation.mutateAsync).toHaveBeenCalledWith('stored-file-old');
  });

  test('removes carers and children from the household', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Remove Sam Taylor' }));
    await user.click(screen.getByRole('button', { name: 'Remove Mia Rivera' }));

    // Assert
    await waitFor(() => {
      expect(removeHouseholdCarerMutation.mutateAsync).toHaveBeenCalledWith('carer-caregiver');
    });
    expect(removeHouseholdChildMutation.mutateAsync).toHaveBeenCalledWith('child-1');
  });
});

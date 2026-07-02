import { QueryClientProvider } from '@tanstack/react-query';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { ReactNode } from 'react';
import { createTestQueryClient } from '../../../testUtils/testQueryClientHelpers';
import type { ChildModel, HouseholdModel, SaveChildForm } from '../types/household.types';
import { ChildForm } from './ChildForm';

const household: HouseholdModel = {
  id: 'household-1',
  name: 'Rivera Household',
  carers: [],
  children: [],
  invitations: []
};

const child: ChildModel = {
  id: 'child-1',
  firstName: 'Mia',
  lastName: 'Rivera',
  dateOfBirth: '2021-04-15',
  avatarStoredFileId: null
};

const renderWithProviders = (children: ReactNode) => {
  const queryClient = createTestQueryClient();
  return render(<QueryClientProvider client={queryClient}>{children}</QueryClientProvider>);
};

describe('ChildForm', () => {
  const onHide = vi.fn<() => void>();
  const onSave = vi.fn<(form: SaveChildForm) => void>();

  beforeEach(() => {
    onHide.mockClear();
    onSave.mockClear();
    vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:child-avatar-preview');
    vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => undefined);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  test('renders the add child form for a household', () => {
    // Act
    renderWithProviders(<ChildForm child={null} household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Assert
    expect(screen.getByRole('dialog', { name: 'Add child in Rivera Household' })).toBeInTheDocument();
    expect(screen.getByLabelText(/first name/i)).toHaveValue('');
    expect(screen.getByLabelText(/last name/i)).toHaveValue('');
    expect(screen.getByLabelText(/date of birth/i)).toHaveValue('');
    expect(screen.getByRole('button', { name: 'Add child' })).toBeDisabled();
  });

  test('shows validation messages when invalid fields are touched', async () => {
    // Arrange
    const user = userEvent.setup();

    renderWithProviders(<ChildForm child={child} household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.clear(screen.getByLabelText(/first name/i));
    await user.clear(screen.getByLabelText(/last name/i));
    await user.clear(screen.getByLabelText(/date of birth/i));

    // Assert
    expect(screen.getByText('First name is required.')).toBeInTheDocument();
    expect(screen.getByText('Last name is required.')).toBeInTheDocument();
    expect(screen.getByText('Date of birth is required.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Save child' })).toBeDisabled();
    expect(onSave).not.toHaveBeenCalled();
  });

  test('submits the completed add child form', async () => {
    // Arrange
    const user = userEvent.setup();
    const avatarFile = new File(['avatar'], 'avatar.png', { type: 'image/png' });

    renderWithProviders(<ChildForm child={null} household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.type(screen.getByLabelText(/first name/i), 'Mia');
    await user.type(screen.getByLabelText(/last name/i), 'Rivera');
    await user.type(screen.getByLabelText(/date of birth/i), '2021-04-15');
    await user.upload(screen.getByLabelText('Upload image'), avatarFile);
    await user.click(screen.getByRole('button', { name: 'Add child' }));

    // Assert
    expect(onSave).toHaveBeenCalledTimes(1);
    expect(onSave).toHaveBeenCalledWith({
      child: {
        firstName: 'Mia',
        lastName: 'Rivera',
        dateOfBirth: '2021-04-15',
        avatarStoredFileId: null
      },
      avatarFile
    } satisfies SaveChildForm);
  });

  test('renders an existing child and submits edits', async () => {
    // Arrange
    const user = userEvent.setup();

    renderWithProviders(<ChildForm child={child} household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.clear(screen.getByLabelText(/first name/i));
    await user.type(screen.getByLabelText(/first name/i), 'Ava');
    await user.click(screen.getByRole('button', { name: 'Save child' }));

    // Assert
    expect(screen.getByRole('dialog', { name: 'Edit child in Rivera Household' })).toBeInTheDocument();
    expect(onSave).toHaveBeenCalledTimes(1);
    expect(onSave).toHaveBeenCalledWith({
      child: {
        firstName: 'Ava',
        lastName: 'Rivera',
        dateOfBirth: '2021-04-15',
        avatarStoredFileId: null
      },
      avatarFile: null
    } satisfies SaveChildForm);
  });

  test('resets the form and hides it when cancelled', async () => {
    // Arrange
    const user = userEvent.setup();

    renderWithProviders(<ChildForm child={child} household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    await user.clear(screen.getByLabelText(/first name/i));

    // Act
    await user.click(screen.getByRole('button', { name: 'Cancel' }));

    // Assert
    expect(onHide).toHaveBeenCalledTimes(1);
    expect(onSave).not.toHaveBeenCalled();
    expect(screen.queryByText('First name is required.')).not.toBeInTheDocument();
    expect(screen.getByLabelText(/first name/i)).toHaveValue('Mia');
  });
});

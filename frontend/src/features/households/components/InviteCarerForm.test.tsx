import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { HouseholdModel, InviteCarerForm as InviteCarerFormModel } from '../types/household.types';
import { InviteCarerForm } from './InviteCarerForm';

const household: HouseholdModel = {
  id: 'household-1',
  name: 'Rivera Household',
  carers: [],
  children: [],
  invitations: []
};

const getRoleField = () => screen.getAllByLabelText(/role/i)[0];

describe('InviteCarerForm', () => {
  const onHide = vi.fn<() => void>();
  const onSave = vi.fn<(form: InviteCarerFormModel) => void>();

  beforeEach(() => {
    onHide.mockClear();
    onSave.mockClear();
  });

  test('renders the invite form for a household', () => {
    // Act
    render(<InviteCarerForm household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Assert
    expect(screen.getByRole('dialog', { name: 'Invite to Rivera Household' })).toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toHaveValue('');
    expect(getRoleField()).toHaveValue('Caregiver');
    expect(screen.getByRole('button', { name: 'Invite' })).toBeEnabled();
  });

  test('reveals validation errors when submitted with invalid values', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<InviteCarerForm household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.click(screen.getByRole('button', { name: 'Invite' }));

    // Assert
    expect(screen.getByText('Enter a valid email address.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Invite' })).toBeDisabled();
    expect(onSave).not.toHaveBeenCalled();
  });

  test('shows a validation error when an invalid email is entered', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<InviteCarerForm household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.type(screen.getByLabelText(/email/i), 'not-an-email');

    // Assert
    expect(screen.getByText('Enter a valid email address.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Invite' })).toBeDisabled();
  });

  test('submits a valid invite with the default caregiver role', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<InviteCarerForm household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.type(screen.getByLabelText(/email/i), 'caregiver@example.com');
    await user.click(screen.getByRole('button', { name: 'Invite' }));

    // Assert
    expect(onSave).toHaveBeenCalledTimes(1);
    expect(onSave).toHaveBeenCalledWith({
      email: 'caregiver@example.com',
      role: 'caregiver'
    } satisfies InviteCarerFormModel);
  });

  test('submits a valid invite with the selected viewer role', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<InviteCarerForm household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.type(screen.getByLabelText(/email/i), 'viewer@example.com');
    screen.getAllByLabelText(/role/i)[0].focus();
    await user.keyboard('v{Enter}');
    await user.click(screen.getByRole('button', { name: 'Invite' }));

    // Assert
    expect(onSave).toHaveBeenCalledTimes(1);
    expect(onSave).toHaveBeenCalledWith({
      email: 'viewer@example.com',
      role: 'viewer'
    } satisfies InviteCarerFormModel);
  });

  test('shows the saving state and disables submit while saving', () => {
    // Act
    render(<InviteCarerForm household={household} saving visible onHide={onHide} onSave={onSave} />);

    // Assert
    expect(screen.getByRole('button', { name: 'Inviting...' })).toBeDisabled();
  });

  test('resets the form and hides it when cancelled', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<InviteCarerForm household={household} saving={false} visible onHide={onHide} onSave={onSave} />);

    await user.type(screen.getByLabelText(/email/i), 'not-an-email');

    // Act
    await user.click(screen.getByRole('button', { name: 'Cancel' }));

    // Assert
    expect(onHide).toHaveBeenCalledTimes(1);
    expect(onSave).not.toHaveBeenCalled();
    expect(screen.queryByText('Enter a valid email address.')).not.toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toHaveValue('');
  });
});

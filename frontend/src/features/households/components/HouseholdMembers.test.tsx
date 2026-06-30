import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { ChildModel, HouseholdModel } from '../types/household.types';
import { HouseholdMembers } from './HouseholdMembers';

const child: ChildModel = {
  id: 'child-1',
  firstName: 'Mia',
  lastName: 'Rivera',
  dateOfBirth: '2021-04-15',
  avatarStoredFileId: null
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

describe('HouseholdMembers', () => {
  const getStatusTone = vi.fn<(status: string) => 'success' | 'warning' | 'neutral'>();
  const onEditChild = vi.fn<(child: ChildModel) => void>();
  const onRemoveCarer = vi.fn<(carerId: string) => void>();
  const onRemoveChild = vi.fn<(childId: string) => void>();

  beforeEach(() => {
    getStatusTone.mockReset();
    getStatusTone.mockReturnValue('neutral');
    onEditChild.mockClear();
    onRemoveCarer.mockClear();
    onRemoveChild.mockClear();
  });

  const renderMembers = (overrides: Partial<Parameters<typeof HouseholdMembers>[0]> = {}) =>
    render(<HouseholdMembers household={household} getStatusTone={getStatusTone} isRemovingCarer={false} isRemovingChild={false} onEditChild={onEditChild} onRemoveCarer={onRemoveCarer} onRemoveChild={onRemoveChild} {...overrides} />);

  test('renders carers, children, and invitations', () => {
    // Arrange

    // Act
    renderMembers();

    // Assert
    expect(screen.getByRole('heading', { name: 'Household Members' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Olivia Rivera' })).toBeInTheDocument();
    expect(screen.getByText('owner@example.com')).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Sam Taylor' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Mia Rivera' })).toBeInTheDocument();
    expect(screen.getByText(/Born 15 Apr 2021/)).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'viewer@example.com' })).toBeInTheDocument();
    expect(screen.getByText(/Expires 01 July 2026/)).toBeInTheDocument();
  });

  test('calls member action handlers with the selected ids and child', async () => {
    // Arrange
    const user = userEvent.setup();

    renderMembers();

    // Act
    await user.click(screen.getByRole('button', { name: 'Remove Sam Taylor' }));
    await user.click(screen.getByRole('button', { name: 'Edit Mia Rivera' }));
    await user.click(screen.getByRole('button', { name: 'Remove Mia Rivera' }));

    // Assert
    expect(onRemoveCarer).toHaveBeenCalledWith('carer-caregiver');
    expect(onEditChild).toHaveBeenCalledWith(child);
    expect(onRemoveChild).toHaveBeenCalledWith('child-1');
  });

  test('does not render a remove action for the owner', () => {
    // Arrange

    // Act
    renderMembers();

    // Assert
    expect(screen.queryByRole('button', { name: 'Remove Olivia Rivera' })).not.toBeInTheDocument();
  });

  test('disables remove actions while removals are in progress', () => {
    // Arrange

    // Act
    renderMembers({ isRemovingCarer: true, isRemovingChild: true });

    // Assert
    expect(screen.getByRole('button', { name: 'Remove Sam Taylor' })).toBeDisabled();
    expect(screen.getByRole('button', { name: 'Remove Mia Rivera' })).toBeDisabled();
  });

  test('renders an empty member state', () => {
    // Arrange
    const emptyHousehold: HouseholdModel = {
      ...household,
      carers: [],
      children: [],
      invitations: []
    };

    // Act
    renderMembers({ household: emptyHousehold });

    // Assert
    expect(screen.getByText('No members are attached to this household yet.')).toBeInTheDocument();
  });
});

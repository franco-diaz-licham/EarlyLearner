import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useState } from 'react';
import type { HouseholdModel } from '../types/household.types';
import { HouseholdHeader } from './HouseholdHeader';

const household: HouseholdModel = {
  id: 'household-1',
  name: 'Rivera Household',
  carers: [],
  children: [],
  invitations: []
};

describe('HouseholdHeader', () => {
  const onAddChild = vi.fn<() => void>();
  const onCancelRename = vi.fn<() => void>();
  const onInviteCarer = vi.fn<() => void>();
  const onNameDraftChange = vi.fn<(name: string) => void>();
  const onSaveRename = vi.fn<() => Promise<void>>();
  const onStartRename = vi.fn<(household: HouseholdModel) => void>();

  beforeEach(() => {
    onAddChild.mockClear();
    onCancelRename.mockClear();
    onInviteCarer.mockClear();
    onNameDraftChange.mockClear();
    onSaveRename.mockReset();
    onSaveRename.mockResolvedValue(undefined);
    onStartRename.mockClear();
  });

  const renderHeader = (overrides: Partial<Parameters<typeof HouseholdHeader>[0]> = {}) =>
    render(
      <HouseholdHeader
        household={household}
        isEditingName={false}
        isError={false}
        isLoading={false}
        isSavingName={false}
        nameDraft="Rivera Household"
        onAddChild={onAddChild}
        onCancelRename={onCancelRename}
        onInviteCarer={onInviteCarer}
        onNameDraftChange={onNameDraftChange}
        onSaveRename={onSaveRename}
        onStartRename={onStartRename}
        {...overrides}
      />
    );

  const StatefulEditingHeader = () => {
    const [nameDraft, setNameDraft] = useState('Rivera');

    const handleNameDraftChange = (nextNameDraft: string) => {
      setNameDraft(nextNameDraft);
      onNameDraftChange(nextNameDraft);
    };

    return (
      <HouseholdHeader
        household={household}
        isEditingName
        isError={false}
        isLoading={false}
        isSavingName={false}
        nameDraft={nameDraft}
        onAddChild={onAddChild}
        onCancelRename={onCancelRename}
        onInviteCarer={onInviteCarer}
        onNameDraftChange={handleNameDraftChange}
        onSaveRename={onSaveRename}
        onStartRename={onStartRename}
      />
    );
  };

  test('renders the household name and primary actions', async () => {
    // Arrange
    const user = userEvent.setup();

    renderHeader();

    // Act
    await user.click(screen.getByRole('button', { name: 'Add child' }));
    await user.click(screen.getByRole('button', { name: 'Invite carer' }));

    // Assert
    expect(screen.getByRole('heading', { name: 'Rivera Household' })).toBeInTheDocument();
    expect(onAddChild).toHaveBeenCalledTimes(1);
    expect(onInviteCarer).toHaveBeenCalledTimes(1);
  });

  test('starts renaming the current household', async () => {
    // Arrange
    const user = userEvent.setup();

    renderHeader();

    // Act
    await user.click(screen.getByRole('button', { name: 'Rename Rivera Household' }));

    // Assert
    expect(onStartRename).toHaveBeenCalledTimes(1);
    expect(onStartRename).toHaveBeenCalledWith(household);
  });

  test('renders loading, error, and empty states', () => {
    // Arrange

    // Act
    const { rerender } = renderHeader({ household: null, isLoading: true });

    // Assert
    expect(screen.getByText('Loading household...')).toBeInTheDocument();

    // Act
    rerender(
      <HouseholdHeader
        household={null}
        isEditingName={false}
        isError
        isLoading={false}
        isSavingName={false}
        nameDraft=""
        onAddChild={onAddChild}
        onCancelRename={onCancelRename}
        onInviteCarer={onInviteCarer}
        onNameDraftChange={onNameDraftChange}
        onSaveRename={onSaveRename}
        onStartRename={onStartRename}
      />
    );

    // Assert
    expect(screen.getByText('Household could not be loaded.')).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'No household yet' })).toBeInTheDocument();
  });

  test('edits the household name and saves with the save button', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<StatefulEditingHeader />);

    // Act
    await user.type(screen.getByLabelText('Household name'), ' Family');
    await user.click(screen.getByRole('button', { name: 'Save' }));

    // Assert
    expect(onNameDraftChange).toHaveBeenLastCalledWith('Rivera Family');
    expect(onSaveRename).toHaveBeenCalledTimes(1);
  });

  test('saves or cancels rename from the keyboard', async () => {
    // Arrange
    const user = userEvent.setup();

    renderHeader({ isEditingName: true, nameDraft: 'Rivera' });
    const nameInput = screen.getByLabelText('Household name');

    // Act
    await user.type(nameInput, '{Enter}');
    await user.type(nameInput, '{Escape}');

    // Assert
    expect(onSaveRename).toHaveBeenCalledTimes(1);
    expect(onCancelRename).toHaveBeenCalledTimes(1);
  });

  test('disables the rename save button while saving', () => {
    // Arrange

    // Act
    renderHeader({ isEditingName: true, isSavingName: true, nameDraft: 'Rivera' });

    // Assert
    expect(screen.getByRole('button', { name: 'Saving...' })).toBeDisabled();
  });
});

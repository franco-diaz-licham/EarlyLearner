import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { ChildModel } from '../../households/types/household.types';
import type { LearningLogFormModel } from '../types/dailyLog.types';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';
import { LearningLogForm } from './LearningLogForm';

const child: ChildModel = {
  id: 'child-1',
  firstName: 'Mia',
  lastName: 'Rivera',
  dateOfBirth: '2021-04-15',
  avatarStoredFileId: null
};

const learningOutcome: LearningOutcomeModel = {
  learningOutcomeId: 'outcome-1',
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10,
  status: LearningOutcomeStatus.Active
};

describe('LearningLogForm', () => {
  const onHide = vi.fn<() => void>();
  const onSave = vi.fn<(form: LearningLogFormModel) => void>();

  beforeEach(() => {
    onHide.mockClear();
    onSave.mockClear();
  });

  test('renders the add learning log form', () => {
    // Act
    render(<LearningLogForm children={[child]} learningOutcomes={[learningOutcome]} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Assert
    expect(screen.getByRole('dialog', { name: 'Add learning log' })).toBeInTheDocument();
    expect(screen.getByLabelText(/child/i)).toHaveValue('Mia Rivera');
    expect(screen.getByLabelText(/date/i)).toHaveValue(new Date().toISOString().slice(0, 10));
    expect(screen.getByLabelText(/type/i)).toHaveValue('Activity');
    expect(screen.getByLabelText(/title/i)).toHaveValue('');
    expect(screen.getByLabelText(/notes/i)).toHaveValue('');
    expect(screen.getByLabelText(/listens and responds/i)).not.toBeChecked();
    expect(screen.getByRole('button', { name: 'Save activity' })).toBeDisabled();
  });

  test('shows validation messages when invalid fields are touched', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<LearningLogForm children={[child]} learningOutcomes={[learningOutcome]} saving={false} visible onHide={onHide} onSave={onSave} />);

    await user.type(screen.getByLabelText(/title/i), 'Paint mixing');
    await user.type(screen.getByLabelText(/notes/i), 'Mixed colours and described the changes.');

    // Act
    await user.clear(screen.getByLabelText(/title/i));
    await user.clear(screen.getByLabelText(/notes/i));

    // Assert
    expect(screen.getByText('Title is required.')).toBeInTheDocument();
    expect(screen.getByText('Notes are required.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Save activity' })).toBeDisabled();
    expect(onSave).not.toHaveBeenCalled();
  });

  test('submits a completed learning log', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<LearningLogForm children={[child]} learningOutcomes={[learningOutcome]} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.type(screen.getByLabelText(/title/i), 'Paint mixing');
    await user.type(screen.getByLabelText(/notes/i), 'Mixed colours and described the changes.');
    await user.click(screen.getByLabelText(/listens and responds/i));
    await user.click(screen.getByRole('button', { name: 'Save activity' }));

    // Assert
    expect(onSave).toHaveBeenCalledTimes(1);
    expect(onSave).toHaveBeenCalledWith({
      childId: 'child-1',
      logDate: new Date().toISOString().slice(0, 10),
      kind: 'activity',
      title: 'Paint mixing',
      notes: 'Mixed colours and described the changes.',
      learningOutcomeIds: ['outcome-1']
    } satisfies LearningLogFormModel);
  });

  test('resets the form and hides it when cancelled', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<LearningLogForm children={[child]} learningOutcomes={[learningOutcome]} saving={false} visible onHide={onHide} onSave={onSave} />);

    await user.type(screen.getByLabelText(/title/i), 'Paint mixing');

    // Act
    await user.click(screen.getByRole('button', { name: 'Cancel' }));

    // Assert
    expect(onHide).toHaveBeenCalledTimes(1);
    expect(onSave).not.toHaveBeenCalled();
    expect(screen.getByLabelText(/title/i)).toHaveValue('');
  });
});

import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { LearningOutcomeFormModel } from '../hooks/useLearningOutcomeForm';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';
import { LearningOutcomeForm } from './LearningOutcomeForm';

const learningOutcome: LearningOutcomeModel = {
  learningOutcomeId: 'outcome-1',
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10,
  status: LearningOutcomeStatus.Active
};

describe('LearningOutcomeForm', () => {
  const onHide = vi.fn<() => void>();
  const onSave = vi.fn<(form: LearningOutcomeFormModel) => void>();

  beforeEach(() => {
    onHide.mockClear();
    onSave.mockClear();
  });

  test('renders the add learning outcome form', () => {
    // Act
    render(<LearningOutcomeForm saving={false} visible onHide={onHide} onSave={onSave} />);

    // Assert
    expect(screen.getByRole('dialog', { name: 'Add learning outcome' })).toBeInTheDocument();
    expect(screen.getByLabelText(/code/i)).toHaveValue('');
    expect(screen.getByLabelText(/name/i)).toHaveValue('');
    expect(screen.getByLabelText(/category/i)).toHaveValue('');
    expect(screen.getByLabelText(/description/i)).toHaveValue('');
    expect(screen.getByRole('button', { name: 'Add outcome' })).toBeDisabled();
  });

  test('shows validation messages when invalid fields are touched', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<LearningOutcomeForm outcome={learningOutcome} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.clear(screen.getByLabelText(/name/i));
    await user.clear(screen.getByLabelText(/category/i));
    await user.clear(screen.getByLabelText(/description/i));

    // Assert
    expect(screen.getByText('Name is required.')).toBeInTheDocument();
    expect(screen.getByText('Category is required.')).toBeInTheDocument();
    expect(screen.getByText('Description is required.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Save outcome' })).toBeDisabled();
    expect(onSave).not.toHaveBeenCalled();
  });

  test('submits the completed add learning outcome form', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<LearningOutcomeForm saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.type(screen.getByLabelText(/code/i), 'language-listening');
    await user.type(screen.getByLabelText(/name/i), 'Listens and responds');
    await user.type(screen.getByLabelText(/category/i), 'Language');
    await user.clear(screen.getByLabelText(/order/i));
    await user.type(screen.getByLabelText(/order/i), '10');
    await user.type(screen.getByLabelText(/description/i), 'Responds to familiar sounds, words, and stories.');
    await user.click(screen.getByRole('button', { name: 'Add outcome' }));

    // Assert
    expect(onSave).toHaveBeenCalledTimes(1);
    expect(onSave).toHaveBeenCalledWith({
      code: 'language-listening',
      name: 'Listens and responds',
      description: 'Responds to familiar sounds, words, and stories.',
      category: 'Language',
      sortOrder: 10
    } satisfies LearningOutcomeFormModel);
  });

  test('renders an existing learning outcome and submits edits', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<LearningOutcomeForm outcome={learningOutcome} saving={false} visible onHide={onHide} onSave={onSave} />);

    // Act
    await user.clear(screen.getByLabelText(/name/i));
    await user.type(screen.getByLabelText(/name/i), 'Updated outcome');
    await user.click(screen.getByRole('button', { name: 'Save outcome' }));

    // Assert
    expect(screen.getByRole('dialog', { name: 'Edit learning outcome' })).toBeInTheDocument();
    expect(screen.getByLabelText(/code/i)).toBeDisabled();
    expect(onSave).toHaveBeenCalledTimes(1);
    expect(onSave).toHaveBeenCalledWith({
      code: 'language-listening',
      name: 'Updated outcome',
      description: 'Responds to familiar sounds, words, and stories.',
      category: 'Language',
      sortOrder: 10
    } satisfies LearningOutcomeFormModel);
  });

  test('resets the form and hides it when cancelled', async () => {
    // Arrange
    const user = userEvent.setup();

    render(<LearningOutcomeForm outcome={learningOutcome} saving={false} visible onHide={onHide} onSave={onSave} />);

    await user.clear(screen.getByLabelText(/name/i));

    // Act
    await user.click(screen.getByRole('button', { name: 'Cancel' }));

    // Assert
    expect(onHide).toHaveBeenCalledTimes(1);
    expect(onSave).not.toHaveBeenCalled();
    expect(screen.queryByText('Name is required.')).not.toBeInTheDocument();
    expect(screen.getByLabelText(/name/i)).toHaveValue('Listens and responds');
  });
});
